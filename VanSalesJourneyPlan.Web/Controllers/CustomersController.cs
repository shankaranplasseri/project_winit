using Microsoft.AspNetCore.Mvc;
using VanSalesJourneyPlan.Web.Models;
using VanSalesJourneyPlan.Web.Services;

namespace VanSalesJourneyPlan.Web.Controllers;

public class CustomersController : Controller
{
    private readonly IApiClient _apiClient;

    public CustomersController(IApiClient apiClient)
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
        return View(customers ?? new List<CustomerDto>());
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (!EnsureAuthenticated())
            return RedirectToAction("Login", "Account");

        return View(new CustomerDto());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CustomerDto model)
    {
        if (!EnsureAuthenticated())
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
            return View(model);

        var result = await _apiClient.PostAsync<CustomerDto>("customers", model);
        
        if (result != null)
        {
            TempData["Success"] = "Customer created successfully";
            return RedirectToAction("Index");
        }

        ModelState.AddModelError("", "Failed to create customer");
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (!EnsureAuthenticated())
            return RedirectToAction("Login", "Account");

        var customer = await _apiClient.GetAsync<CustomerDto>($"customers/{id}");
        if (customer == null)
            return NotFound();

        return View(customer);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, CustomerDto model)
    {
        if (!EnsureAuthenticated())
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
            return View(model);

        model.CustomerId = id;
        var result = await _apiClient.PutAsync<CustomerDto>($"customers/{id}", model);

        if (result != null)
        {
            TempData["Success"] = "Customer updated successfully";
            return RedirectToAction("Index");
        }

        ModelState.AddModelError("", "Failed to update customer");
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        if (!EnsureAuthenticated())
            return RedirectToAction("Login", "Account");

        var success = await _apiClient.DeleteAsync($"customers/{id}");
        
        if (success)
            TempData["Success"] = "Customer deleted successfully";
        else
            TempData["Error"] = "Failed to delete customer";

        return RedirectToAction("Index");
    }
}
