using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using api_doc.Models;
using api_doc.Models.Artwork;
using api_doc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_doc.Controllers;

[Authorize]
public class ArtworkController : Controller
{
    // GET

    readonly IApiClient _api;

    public ArtworkController(IApiClient api)
    {
        _api = api;
    }
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var items = await _api.GetListAsync<ArtworkListItemDto>("artwork", ct: ct);

        ViewBag.ApiHost = new Uri(_api.Http.BaseAddress!, "/").GetLeftPart(UriPartial.Authority);
        return View(items);
    }

    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var vm = new ArtworkCreateViewModel
        {
            Materials = await _api.GetListAsync<MaterialReponseDto>("artwork/materials", ct: ct),
            Techniques = await _api.GetListAsync<TechniqueReponseDto>("artwork/tecnique", ct: ct),
            ArtTypes = await _api.GetListAsync<ArtTypeReponseDto>("artwork/type", ct: ct),
            Statuses = await _api.GetListAsync<ArtStatusReponseDto>("artwork/status", ct: ct),
            OrientationTypes = await _api.GetListAsync<OrientationTypeReponseDto>("orientation-type", ct: ct),
            Languages = await _api.GetListAsync<LanguageResponseDto>("accessibilitie/language", ct: ct),
            ResourceType = await _api.GetListAsync<ResourceTypeResponseDto>("accessibilities/resource", ct: ct)
        };

        return View(vm);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(300_000_000)]
    [RequestFormLimits(MultipartBodyLengthLimit = 300_000_000, ValueCountLimit = int.MaxValue)]
    public async Task<IActionResult> Store(CancellationToken ct)
    {
        try
        {
            var f = Request.Form;

            static void AddString(MultipartFormDataContent mp, string name, string? value)
            {
                if (value is null) return;
                mp.Add(new StringContent(value, Encoding.UTF8), name);
            }

            string? creationDate = f["creationDate"];
            if (!string.IsNullOrWhiteSpace(creationDate))
            {
                if (DateTime.TryParse(creationDate, out var dt))
                    creationDate = dt.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
            }

            var mp = new MultipartFormDataContent();

            // Campos simples
            AddString(mp, "title", f["title"]);
            AddString(mp, "description", f["description"]);
            AddString(mp, "creationDate", creationDate);
            AddString(mp, "price", f["price"]);
            AddString(mp, "location", f["location"]);
            AddString(mp, "weight", f["weight"]);
            AddString(mp, "dimensions", f["dimensions"]);
            AddString(mp, "stock", f["stock"]);
            AddString(mp, "materialId", f["materialId"]);
            AddString(mp, "artTypeId", f["artTypeId"]);
            AddString(mp, "statusId", f["statusId"]);
            AddString(mp, "orientationType", f["orientationType"]);


            foreach (var t in f["techniqueIds"])
                AddString(mp, "techniqueIds", t);


            var acc = new[]
            {
                new
                {
                    AccessibilityDescription = (string?)f["accessibilityDescription"] ?? "",
                    CreateDate = DateTime.UtcNow,
                    ResourceTypeId = int.TryParse(f["resourceTypeId"], out var rId) ? rId : 0,
                    LanguageId = int.TryParse(f["languageId"], out var lId) ? lId : 0
                }
            };
            var accJson = JsonSerializer.Serialize(acc);
            mp.Add(new StringContent(accJson, Encoding.UTF8, "application/json"), "accessibilities");
            mp.Add(new StringContent(accJson, Encoding.UTF8, "text/plain"), "accessibilities");

            var obj3d = Request.Form.Files["object3DFile"];
            var image = Request.Form.Files["imageFile"];

            if (obj3d is { Length: > 0 })
            {
                var sc = new StreamContent(obj3d.OpenReadStream());
                sc.Headers.ContentType = new MediaTypeHeaderValue(obj3d.ContentType ?? "application/octet-stream");
                mp.Add(sc, "object3DFile", obj3d.FileName);
            }

            if (image is { Length: > 0 })
            {
                var sc = new StreamContent(image.OpenReadStream());
                sc.Headers.ContentType = new MediaTypeHeaderValue(image.ContentType ?? "application/octet-stream");
                mp.Add(sc, "imageFile", image.FileName);
            }

            var result = await _api.PostMultipartAsync<object>("artwork", mp, ct);

            TempData["toast"] = "Obra cadastrada com sucesso.";
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            // Tenta ler o JSON de erro retornado
            try
            {
                var errorJson = ex.Message.Split("::").LastOrDefault();
                if (!string.IsNullOrWhiteSpace(errorJson))
                {
                    using var doc = JsonDocument.Parse(errorJson);
                    if (doc.RootElement.TryGetProperty("errors", out var errors))
                    {
                        foreach (var prop in errors.EnumerateObject())
                        {
                            var field = prop.Name;
                            var messages = prop.Value.EnumerateArray().Select(e => e.GetString());
                            foreach (var msg in messages)
                                ModelState.AddModelError(field, msg ?? "Erro de validação.");
                        }
                    }
                }
            }
            catch
            {
                ModelState.AddModelError("", "Erro de validação no servidor.");
            }

            var vm = new ArtworkCreateViewModel
            {
                Materials = await _api.GetListAsync<MaterialReponseDto>("artwork/materials", ct: ct),
                Techniques = await _api.GetListAsync<TechniqueReponseDto>("artwork/tecnique", ct: ct),
                ArtTypes = await _api.GetListAsync<ArtTypeReponseDto>("artwork/type", ct: ct),
                Statuses = await _api.GetListAsync<ArtStatusReponseDto>("artwork/status", ct: ct),
                OrientationTypes = await _api.GetListAsync<OrientationTypeReponseDto>("orientation-type", ct: ct),
                Languages = await _api.GetListAsync<LanguageResponseDto>("accessibilitie/language", ct: ct),
                ResourceType = await _api.GetListAsync<ResourceTypeResponseDto>("accessibilities/resource", ct: ct)
            };

            return View("Create", vm);
        }
    }
}
