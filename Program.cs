using api_doc.Infrastructure.Vite;
var builder = WebApplication.CreateBuilder(args);


var viteOpts = new ViteManifestOptions
{
    Entry = "src/main.ts",
    DevServerUrl = "http://localhost:5173",
    ManifestFile = "manifest.json"
};

builder.Services.AddSingleton(viteOpts);
builder.Services.AddSingleton<IViteManifestService, ViteManifestService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
