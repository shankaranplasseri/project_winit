using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using VanSalesJourneyPlan.Web.Models;
using VanSalesJourneyPlan.Web.Services;

namespace VanSalesJourneyPlan.Web.Controllers;

public class DashboardController : Controller
{
    private readonly IApiClient _apiClient;

    public DashboardController(IApiClient apiClient)
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

        var customers = await _apiClient.GetAsync<List<CustomerDto>>("customers");
        var plans = await _apiClient.GetAsync<List<JourneyPlanDto>>("journeyplans");

        var stats = new Dashboard
        {
            TotalCustomers = customers?.Count ?? 0,
            TotalPlans = plans?.Count ?? 0,
            CustomersOnRoute = customers?.GroupBy(c => c.Route).Count() ?? 0
        };

        return View(stats);
    }
}
