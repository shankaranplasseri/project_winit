namespace VanSalesJourneyPlan.API.DTOs;

public class JourneyPlanDto
{
    public int JourneyPlanId { get; set; }
    public int AssignedUserId { get; set; }
    public string? AssignedUserName { get; set; }
    public string PlanDate { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<JourneyPlanItemDto> Items { get; set; } = new();
}

public class JourneyPlanItemDto
{
    public int JourneyPlanItemId { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerCode { get; set; }
    public string? CustomerName { get; set; }
    public int SequenceNumber { get; set; }
    public string? Notes { get; set; }
    public string? PlannedVisitTime { get; set; }
    public bool IsCompleted { get; set; }
}

public class CreateJourneyPlanRequest
{
    public int AssignedUserId { get; set; }
    public string PlanDate { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<int> CustomerIds { get; set; } = new();
}

public class AssignJourneyPlanRequest
{
    public int UserId { get; set; }
    public string PlanDate { get; set; } = string.Empty;
}

public class UpdateJourneyPlanRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
}
