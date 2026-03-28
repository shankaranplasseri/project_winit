namespace VanSalesJourneyPlan.Web.Models;

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string? Token { get; set; }
    public UserDto? User { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
}

public class UserDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class CustomerDto
{
    public int CustomerId { get; set; }
    public string CustomerCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public double LocationLatitude { get; set; }
    public double LocationLongitude { get; set; }
    public string Route { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class JourneyPlanItemDto
{
    public int JourneyPlanItemId { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public int Sequence { get; set; }
    public string? Notes { get; set; }
    public string? PlannedVisitTime { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? VisitedDate { get; set; }
}

public class JourneyPlanDto
{
    public int JourneyPlanId { get; set; }
    public int AssignedUserId { get; set; }
    public DateTime? ScheduleDate { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public List<JourneyPlanItemDto>? Items { get; set; } = new();
}

public class CreateJourneyPlanRequest
{
    public int AssignedUserId { get; set; }
    public DateTime ScheduleDate { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<JourneyPlanItemRequest>? Items { get; set; } = new();
}

public class JourneyPlanItemRequest
{
    public int CustomerId { get; set; }
    public int Sequence { get; set; }
}

public class ApiResponse<T>
{
    public T? Data { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
}
