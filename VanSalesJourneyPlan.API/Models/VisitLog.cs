namespace VanSalesJourneyPlan.API.Models;

public class VisitLog
{
    public int VisitLogId { get; set; }
    public int JourneyPlanItemId { get; set; }
    public DateOnly VisitDate { get; set; }
    public TimeOnly? VisitTime { get; set; }
    public string? Notes { get; set; }
    public decimal? SalesAmount { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Completed, Cancelled
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public JourneyPlanItem? JourneyPlanItem { get; set; }
}
