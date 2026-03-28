namespace VanSalesJourneyPlan.API.Models;

public class JourneyPlanItem
{
    public int JourneyPlanItemId { get; set; }
    public int JourneyPlanId { get; set; }
    public int CustomerId { get; set; }
    public int SequenceNumber { get; set; }
    public string? Notes { get; set; }
    public TimeOnly? PlannedVisitTime { get; set; }
    public bool IsCompleted { get; set; } = false;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public JourneyPlan? JourneyPlan { get; set; }
    public Customer? Customer { get; set; }
    public ICollection<VisitLog> VisitLogs { get; set; } = new List<VisitLog>();
}
