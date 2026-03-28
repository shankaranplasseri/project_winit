namespace VanSalesJourneyPlan.API.DTOs;

public class CustomerDto
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
    public string Status { get; set; } = "Active";
}

public class CreateCustomerRequest
{
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
}

public class UpdateCustomerRequest
{
    public string? CustomerName { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? ContactNumber { get; set; }
    public string? Email { get; set; }
    public double? LocationLatitude { get; set; }
    public double? LocationLongitude { get; set; }
    public string? Route { get; set; }
    public string? Status { get; set; }
}
