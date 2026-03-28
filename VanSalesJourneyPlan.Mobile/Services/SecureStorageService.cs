using Microsoft.Maui.Storage;

namespace VanSalesJourneyPlan.Mobile.Services;

public interface ISecureStorageService
{
    Task SaveTokenAsync(string token);
    Task<string?> GetTokenAsync();
    Task RemoveTokenAsync();
    Task SaveUserIdAsync(int userId);
    Task<int?> GetUserIdAsync();
    Task SaveUsernameAsync(string username);
    Task<string?> GetUsernameAsync();
    Task ClearAllAsync();
}

public class SecureStorageService : ISecureStorageService
{
    private const string TokenKey = "jwt_token";
    private const string UserIdKey = "user_id";
    private const string UsernameKey = "username";

    public async Task SaveTokenAsync(string token)
    {
        await SecureStorage.SetAsync(TokenKey, token);
    }

    public async Task<string?> GetTokenAsync()
    {
        try
        {
            return await SecureStorage.GetAsync(TokenKey);
        }
        catch
        {
            return null;
        }
    }

    public async Task RemoveTokenAsync()
    {
        SecureStorage.Remove(TokenKey);
        await Task.CompletedTask;
    }

    public async Task SaveUserIdAsync(int userId)
    {
        await SecureStorage.SetAsync(UserIdKey, userId.ToString());
    }

    public async Task<int?> GetUserIdAsync()
    {
        try
        {
            var value = await SecureStorage.GetAsync(UserIdKey);
            if (int.TryParse(value, out var userId))
            {
                return userId;
            }
        }
        catch { }
        return null;
    }

    public async Task SaveUsernameAsync(string username)
    {
        await SecureStorage.SetAsync(UsernameKey, username);
    }

    public async Task<string?> GetUsernameAsync()
    {
        try
        {
            return await SecureStorage.GetAsync(UsernameKey);
        }
        catch
        {
            return null;
        }
    }

    public async Task ClearAllAsync()
    {
        SecureStorage.Remove(TokenKey);
        SecureStorage.Remove(UserIdKey);
        SecureStorage.Remove(UsernameKey);
        await Task.CompletedTask;
    }
}
