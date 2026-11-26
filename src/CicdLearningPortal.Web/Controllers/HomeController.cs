using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CicdLearningPortal.Web.Models;
using CicdLearningPortal.Web.Services;

namespace CicdLearningPortal.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ILearningCurriculumService _curriculumService;
    private readonly IWebHostEnvironment _environment;

    public HomeController(
        ILogger<HomeController> logger,
        ILearningCurriculumService curriculumService,
        IWebHostEnvironment environment)
    {
        _logger = logger;
        _curriculumService = curriculumService;
        _environment = environment;
    }

    public IActionResult Index()
    {
        var viewModel = new PortalDashboardViewModel
        {
            RuntimeInfo = _curriculumService.GetRuntimeInfo(_environment.EnvironmentName),
            Modules = _curriculumService.GetAllModules()
        };

        return View(viewModel);
    }

    [HttpGet("module/{id}")]
    public IActionResult Module(string id)
    {
        var module = _curriculumService.GetModuleById(id);
        if (module is null)
        {
            return NotFound();
        }

        ViewBag.RuntimeInfo = _curriculumService.GetRuntimeInfo(_environment.EnvironmentName);
        ViewBag.AllModules = _curriculumService.GetAllModules();
        return View(module);
    }

    [HttpGet("docker-guide")]
    public IActionResult DockerGuide()
    {
        var dockerModule = _curriculumService.GetModuleById("docker-fundamentals");
        ViewBag.RuntimeInfo = _curriculumService.GetRuntimeInfo(_environment.EnvironmentName);
        return View(dockerModule);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
