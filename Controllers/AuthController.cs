using System.Security.Claims;
using api_doc.Models;
using api_doc.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace api_doc.Controllers;

public class AuthController : Controller
{
    
    readonly IApiClient _api;

    public AuthController(IApiClient api)
    {
        _api = api;
    }
    
    // GET
    [HttpGet("login")]
    public IActionResult Index([FromQuery] string? returnUrl = null)
    {
        return View( new Login {ReturnUrl = returnUrl});
    }
    
    [HttpPost("Login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(Login vm, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(vm.Email) || string.IsNullOrWhiteSpace(vm.Password))
        {
            ModelState.AddModelError(string.Empty, "Informe e-mail e senha.");
            return View("Index", vm);
        }

        try
        {
            var payload  = new LoginRequestDto { Email = vm.Email!, Password = vm.Password! };
            var resp = await _api.PostAsync<LoginRequestDto, LoginResponseDto>("auth/login", payload, ct);

            if (resp is null || string.IsNullOrWhiteSpace(resp.Token))
            {
                ModelState.AddModelError(string.Empty, $"Falha no login. Verifique suas credenciais.");
                return View("Index", vm);
            }

            var cookieOpts = new CookieOptions
            {
                HttpOnly = true,
                Secure   = false, 
                SameSite = SameSiteMode.Lax,
                Expires  = DateTimeOffset.UtcNow.AddSeconds(Math.Max(resp.ExpiresIn, 3600))
            };
            Response.Cookies.Append("access_token", resp.Token, cookieOpts);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, vm.Email!),
                new(ClaimTypes.Name, vm.Email!)
            };

            var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var authProps = new AuthenticationProperties
            {
                IsPersistent = vm.RememberMe,
                ExpiresUtc   = DateTimeOffset.UtcNow.AddDays(vm.RememberMe ? 7 : 1)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProps);

            var redirectUrl = vm.ReturnUrl;
            if (string.IsNullOrWhiteSpace(redirectUrl) || !Url.IsLocalUrl(redirectUrl))
                redirectUrl = Url.RouteUrl("home") ?? Url.Action("Index", "Home");

            return Redirect(redirectUrl!);
        }
        catch (HttpRequestException ex)
        {
            ModelState.AddModelError(string.Empty, $"Login inválido: {ex.Message}");
            return View("Index", vm);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Erro ao autenticar: {ex.Message}");
            return View("Index", vm);
        }
    }

    [HttpPost("Logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        Response.Cookies.Delete("access_token");
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Index));
    }
}
