namespace VanSalesJourneyPlan.API.Models;

public class Customer
{
    public int CustomerId { get; set; }
    public string CustomerCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? ContactNumber { get; set; }
    public string? Email { get; set; }
    public double? LocationLatitude { get; set; }
    public double? LocationLongitude { get; set; }
    public string? Route { get; set; }
    public string Status { get; set; } = "Active"; // Active, Inactive, Prospect
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<JourneyPlanItem> JourneyPlanItems { get; set; } = new List<JourneyPlanItem>();
}
