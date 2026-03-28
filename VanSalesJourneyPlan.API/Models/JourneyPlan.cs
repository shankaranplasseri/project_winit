namespace VanSalesJourneyPlan.API.Models;

public class JourneyPlan
{
    public int JourneyPlanId { get; set; }
    public int AssignedUserId { get; set; }
    public DateOnly PlanDate { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = "Draft"; // Draft, Active, Completed, Cancelled
    public int CreatedUserId { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User? AssignedUser { get; set; }
    public User? CreatedUser { get; set; }
    public ICollection<JourneyPlanItem> JourneyPlanItems { get; set; } = new List<JourneyPlanItem>();
}
