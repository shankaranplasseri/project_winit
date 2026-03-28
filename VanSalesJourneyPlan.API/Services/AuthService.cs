using BCrypt.Net;
using VanSalesJourneyPlan.API.Data;
using VanSalesJourneyPlan.API.Models;

namespace VanSalesJourneyPlan.API.Services;

public interface IAuthService
{
    Task<(bool success, string message, User? user)> LoginAsync(string username, string password);
    Task<User?> GetUserByIdAsync(int userId);
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(bool success, string message, User? user)> LoginAsync(string username, string password)
    {
        try
        {
            var user = await Task.FromResult(_context.Users.FirstOrDefault(u => u.Username == username && u.IsActive));

            if (user == null)
            {
                return (false, "Invalid username or password", null);
            }

            if (!VerifyPassword(password, user.PasswordHash))
            {
                return (false, "Invalid username or password", null);
            }

            return (true, "Login successful", user);
        }
        catch (Exception ex)
        {
            return (false, $"Login failed: {ex.Message}", null);
        }
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return _context.Users.FirstOrDefault(u => u.UserId == userId && u.IsActive);
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch
        {
            return false;
        }
    }
}
