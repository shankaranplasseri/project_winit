using Microsoft.AspNetCore.Mvc;
using VanSalesJourneyPlan.Web.Models;
using VanSalesJourneyPlan.Web.Services;

namespace VanSalesJourneyPlan.Web.Controllers;

public class JourneyPlansController : Controller
{
    private readonly IApiClient _apiClient;

    public JourneyPlansController(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    private bool EnsureAuthenticated()
    {
        var token = HttpContext.Session.GetString("jwt_token");
        if (string.IsNullOrEmpty(token))
        {
            return false;
        }
        _apiClient.SetToken(token);
        return true;
    }

    public async Task<IActionResult> Index()
    {
        if (!EnsureAuthenticated())
            return RedirectToAction("Login", "Account");

        var plans = await _apiClient.GetAsync<List<JourneyPlanDto>>("journeyplans");
        return View(plans ?? new List<JourneyPlanDto>());
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (!EnsureAuthenticated())
            return RedirectToAction("Login", "Account");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateJourneyPlanRequest model)
    {
        if (!EnsureAuthenticated())
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
            return View(model);

        var result = await _apiClient.PostAsync<JourneyPlanDto>("journeyplans", model);
        
        if (result != null)
        {
            TempData["Success"] = "Journey plan created successfully";
            return RedirectToAction("Details", new { id = result.JourneyPlanId });
        }

        ModelState.AddModelError("", "Failed to create journey plan");
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        if (!EnsureAuthenticated())
            return RedirectToAction("Login", "Account");

        var plan = await _apiClient.GetAsync<JourneyPlanDto>($"journeyplans/{id}");
        if (plan == null)
            return NotFound();

        return View(plan);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (!EnsureAuthenticated())
            return RedirectToAction("Login", "Account");

        var plan = await _apiClient.GetAsync<JourneyPlanDto>($"journeyplans/{id}");
        if (plan == null)
            return NotFound();

        return View(plan);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, JourneyPlanDto model)
    {
        if (!EnsureAuthenticated())
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
            return View(model);

        model.JourneyPlanId = id;
        var result = await _apiClient.PutAsync<JourneyPlanDto>($"journeyplans/{id}", model);

        if (result != null)
        {
            TempData["Success"] = "Journey plan updated successfully";
            return RedirectToAction("Details", new { id = id });
        }

        ModelState.AddModelError("", "Failed to update journey plan");
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        if (!EnsureAuthenticated())
            return RedirectToAction("Login", "Account");

        var success = await _apiClient.DeleteAsync($"journeyplans/{id}");
        
        if (success)
            TempData["Success"] = "Journey plan deleted successfully";
        else
            TempData["Error"] = "Failed to delete journey plan";

        return RedirectToAction("Index");
    }
}
