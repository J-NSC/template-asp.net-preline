using api_doc.Infrastructure.Http;
using api_doc.Infrastructure.Ui;
using api_doc.Infrastructure.Vite;
using api_doc.Option;
using api_doc.Service;
using api_doc.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
var builder = WebApplication.CreateBuilder(args);


var viteOpts = new ViteManifestOptions
{
    Entry = "src/main.ts",
    DevServerUrl = "http://localhost:5173",
    ManifestFile = "manifest.json"
};

builder.WebHost.ConfigureKestrel(o =>
{
    o.Limits.MaxRequestBodySize = 300_000_000;
});

builder.Services.Configure<FormOptions>(o =>
{
    o.MultipartBodyLengthLimit = 300_000_000;   // 300 MB para multipart
    o.ValueCountLimit = int.MaxValue;           // opcional
});



builder.Services.Configure<ApiOption>(builder.Configuration.GetSection(ApiOption.SectionName));

builder.Services.AddHttpContextAccessor();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/Login";
        o.LogoutPath = "/Logout";
        o.AccessDeniedPath = "/AcessoNegado";
        o.SlidingExpiration = true;
        o.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

builder.Services.AddTransient<AuthHeaderHandler>();

builder.Services.AddSingleton(viteOpts);
builder.Services.AddSingleton<IViteManifestService, ViteManifestService>();


builder.Services.AddHttpClient<IApiClient, ApiClient>()
    .AddHttpMessageHandler<AuthHeaderHandler>();

builder.Services.AddScoped<SwaggerDocumentService>();
builder.Services.AddScoped<IViewDataAccessor, ViewDataAccessor>();
builder.Services.AddSingleton<IViewDataAccessor, ViewDataAccessor>();

builder.Services.AddHttpClient<ApiService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7060");
});

builder.Services.AddHttpClient("SwaggerDoc", (sp, c) =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    c.BaseAddress = new Uri(cfg["ApiDoc:SwaggerUrl"]!);
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// pipeline...
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllers();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();