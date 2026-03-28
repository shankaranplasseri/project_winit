namespace VanSalesJourneyPlan.Mobile.Models;

public class JourneyPlanCache
{
    public int JourneyPlanId { get; set; }
    public int AssignedUserId { get; set; }
    public DateTime PlanDate { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string ItemsJson { get; set; } = string.Empty;
}

public class CustomerCache
{
    public int CustomerId { get; set; }
    public string CustomerCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
    public double LocationLatitude { get; set; }
    public double LocationLongitude { get; set; }
    public string Route { get; set; } = string.Empty;
}

public class VisitLogCache
{
    public int VisitLogId { get; set; }
    public int JourneyPlanItemId { get; set; }
    public DateTime VisitDate { get; set; }
    public string VisitTime { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public decimal SalesAmount { get; set; }
    public string Status { get; set; } = string.Empty;
}
