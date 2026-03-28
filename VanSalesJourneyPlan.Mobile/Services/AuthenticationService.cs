using Newtonsoft.Json;
using VanSalesJourneyPlan.Mobile.Models;

namespace VanSalesJourneyPlan.Mobile.Services;

public interface IAuthenticationService
{
    Task<(bool success, string message, User? user)> LoginAsync(string username, string password);
    Task LogoutAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<User?> GetCurrentUserAsync();
}

public class AuthenticationService : IAuthenticationService
{
    private readonly IApiClient _apiClient;
    private readonly ISecureStorageService _secureStorage;

    public AuthenticationService(IApiClient apiClient, ISecureStorageService secureStorage)
    {
        _apiClient = apiClient;
        _secureStorage = secureStorage;
    }

    public async Task<(bool success, string message, User? user)> LoginAsync(string username, string password)
    {
        try
        {
            var loginRequest = new { username, password };
            var response = await _apiClient.PostAsync<LoginResponse>("auth/login", loginRequest);

            if (response?.Success == true && !string.IsNullOrEmpty(response.Token) && response.User != null)
            {
                // Save token and user info
                await _secureStorage.SaveTokenAsync(response.Token);
                await _secureStorage.SaveUserIdAsync(response.User.UserId);
                await _secureStorage.SaveUsernameAsync(response.User.Username);

                // Set token in API client
                _apiClient.SetToken(response.Token);

                return (true, "Login successful", response.User);
            }

            return (false, response?.Message ?? "Login failed", null);
        }
        catch (Exception ex)
        {
            return (false, $"Login error: {ex.Message}", null);
        }
    }

    public async Task LogoutAsync()
    {
        await _apiClient.PostAsync<dynamic>("auth/logout");
        _apiClient.ClearToken();
        await _secureStorage.ClearAllAsync();
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await _secureStorage.GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        var userId = await _secureStorage.GetUserIdAsync();
        var username = await _secureStorage.GetUsernameAsync();

        if (userId.HasValue && !string.IsNullOrEmpty(username))
        {
            return new User
            {
                UserId = userId.Value,
                Username = username
            };
        }

        return null;
    }
}

public interface IJourneyPlanService
{
    Task<(bool success, List<JourneyPlanDto>? plans)> GetMyJourneyPlansAsync();
    Task<(bool success, JourneyPlanDto? plan)> GetJourneyPlanDetailsAsync(int planId);
    Task<(bool success, bool created)> LogVisitAsync(int planItemId, DateTime visitDate, string? visitTime, string? notes, decimal? salesAmount);
}

public class JourneyPlanService : IJourneyPlanService
{
    private readonly IApiClient _apiClient;
    private readonly ILocalCacheService _cacheService;

    public JourneyPlanService(IApiClient apiClient, ILocalCacheService cacheService)
    {
        _apiClient = apiClient;
        _cacheService = cacheService;
    }

    public async Task<(bool success, List<JourneyPlanDto>? plans)> GetMyJourneyPlansAsync()
    {
        try
        {
            var response = await _apiClient.GetAsync<ApiResponse<List<JourneyPlanDto>>>("journeyplans/my");

            if (response?.Success == true && response.Data != null)
            {
                // Cache the plans
                var cachePlans = response.Data.Select(p => new JourneyPlanCache
                {
                    JourneyPlanId = p.JourneyPlanId,
                    AssignedUserId = p.AssignedUserId,
                    AssignedUserName = p.AssignedUserName ?? string.Empty,
                    PlanDate = p.PlanDate,
                    Title = p.Title,
                    Description = p.Description,
                    Status = p.Status,
                    ItemsJson = JsonConvert.SerializeObject(p.Items)
                }).ToList();

                await _cacheService.SaveJourneyPlansAsync(cachePlans);

                return (true, response.Data);
            }

            // Return cached data if API fails
            var cachedPlans = await _cacheService.GetJourneyPlansAsync();
            var result = cachedPlans.Select(cp => new JourneyPlanDto
            {
                JourneyPlanId = cp.JourneyPlanId,
                AssignedUserId = cp.AssignedUserId,
                AssignedUserName = cp.AssignedUserName,
                PlanDate = cp.PlanDate,
                Title = cp.Title,
                Description = cp.Description,
                Status = cp.Status,
                Items = JsonConvert.DeserializeObject<List<JourneyPlanItemDto>>(cp.ItemsJson) ?? new List<JourneyPlanItemDto>()
            }).ToList();

            return (cachedPlans.Count > 0, result.Count > 0 ? result : null);
        }
        catch
        {
            // Return cached data on error
            var cachedPlans = await _cacheService.GetJourneyPlansAsync();
            var result = cachedPlans.Select(cp => new JourneyPlanDto
            {
                JourneyPlanId = cp.JourneyPlanId,
                AssignedUserId = cp.AssignedUserId,
                AssignedUserName = cp.AssignedUserName,
                PlanDate = cp.PlanDate,
                Title = cp.Title,
                Description = cp.Description,
                Status = cp.Status,
                Items = JsonConvert.DeserializeObject<List<JourneyPlanItemDto>>(cp.ItemsJson) ?? new List<JourneyPlanItemDto>()
            }).ToList();

            return (cachedPlans.Count > 0, result.Count > 0 ? result : null);
        }
    }

    public async Task<(bool success, JourneyPlanDto? plan)> GetJourneyPlanDetailsAsync(int planId)
    {
        var response = await _apiClient.GetAsync<ApiResponse<JourneyPlanDto>>($"journeyplans/{planId}");
        return (response?.Success == true, response?.Data);
    }

    public async Task<(bool success, bool created)> LogVisitAsync(int planItemId, DateTime visitDate, string? visitTime, string? notes, decimal? salesAmount)
    {
        try
        {
            var request = new
            {
                journeyPlanItemId = planItemId,
                visitDate = visitDate.ToString("yyyy-MM-dd"),
                visitTime = visitTime,
                notes = notes,
                salesAmount = salesAmount
            };

            var response = await _apiClient.PostAsync<ApiResponse<int>>("visitlogs", request);

            if (response?.Success == true)
            {
                // Cache the visit log
                var log = new VisitLogCache
                {
                    VisitLogId = response.Data,
                    JourneyPlanItemId = planItemId,
                    VisitDate = visitDate.ToString("yyyy-MM-dd"),
                    VisitTime = visitTime,
                    Notes = notes,
                    SalesAmount = salesAmount,
                    Status = "Completed"
                };

                await _cacheService.SaveVisitLogAsync(log);
                return (true, true);
            }

            return (false, false);
        }
        catch
        {
            return (false, false);
        }
    }
}

// DTOs for Mobile
public class LoginResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Token { get; set; }
    public User? User { get; set; }
}

public class User
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Role { get; set; } = string.Empty;
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
}

public class JourneyPlanDto
{
    public int JourneyPlanId { get; set; }
    public int AssignedUserId { get; set; }
    public string? AssignedUserName { get; set; }
    public string PlanDate { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<JourneyPlanItemDto> Items { get; set; } = new();
}

public class JourneyPlanItemDto
{
    public int JourneyPlanItemId { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerCode { get; set; }
    public string? CustomerName { get; set; }
    public int SequenceNumber { get; set; }
    public string? Notes { get; set; }
    public string? PlannedVisitTime { get; set; }
    public bool IsCompleted { get; set; }
}
