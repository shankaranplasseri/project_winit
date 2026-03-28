namespace VanSalesJourneyPlan.API.Models;

public class User
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string Role { get; set; } = "VanSalesUser"; // Admin, VanSalesUser
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<JourneyPlan> AssignedJourneyPlans { get; set; } = new List<JourneyPlan>();
    public ICollection<JourneyPlan> CreatedJourneyPlans { get; set; } = new List<JourneyPlan>();
}
