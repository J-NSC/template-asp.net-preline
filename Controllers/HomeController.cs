using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using api_doc.Models;
using api_doc.Service;
using Microsoft.AspNetCore.Authorization;

namespace api_doc.Controllers;
[Authorize]
public class HomeController : Controller
{
    readonly ILogger<HomeController> _logger;
    readonly ApiService _api;

    public HomeController(ILogger<HomeController> logger, ApiService api)
    {
        _logger = logger;
        _api = api;
    }
    
    [HttpGet("home", Name = "home")]
    public IActionResult Index()
    {
        ViewData["Title"] = "Home";

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
