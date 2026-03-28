using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using VanSalesJourneyPlan.Web.Models;
using VanSalesJourneyPlan.Web.Services;

namespace VanSalesJourneyPlan.Web.Controllers;

public class AccountController : Controller
{
    private readonly IApiClient _apiClient;

    public AccountController(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (HttpContext.Session.GetString("jwt_token") != null)
            return RedirectToAction("Index", "Dashboard");
        
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var response = await _apiClient.PostAsync<LoginResponse>("auth/login", model);

        if (response?.Success == true && !string.IsNullOrEmpty(response.Token))
        {
            // Store token in session
            HttpContext.Session.SetString("jwt_token", response.Token);
            HttpContext.Session.SetString("user", JsonSerializer.Serialize(response.User));

            // Set token for API client
            _apiClient.SetToken(response.Token);

            return RedirectToAction("Index", "Dashboard");
        }

        ModelState.AddModelError("", response?.Message ?? "Login failed");
        return View(model);
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        _apiClient.ClearToken();
        return RedirectToAction("Login");
    }
}
