namespace VanSalesJourneyPlan.API.DTOs;

public class VisitLogDto
{
    public int VisitLogId { get; set; }
    public int JourneyPlanItemId { get; set; }
    public string VisitDate { get; set; } = string.Empty;
    public string? VisitTime { get; set; }
    public string? Notes { get; set; }
    public decimal? SalesAmount { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class CreateVisitLogRequest
{
    public int JourneyPlanItemId { get; set; }
    public string VisitDate { get; set; } = string.Empty;
    public string? VisitTime { get; set; }
    public string? Notes { get; set; }
    public decimal? SalesAmount { get; set; }
}

public class UpdateVisitLogRequest
{
    public string? VisitTime { get; set; }
    public string? Notes { get; set; }
    public decimal? SalesAmount { get; set; }
    public string? Status { get; set; }
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
}

public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string>? Errors { get; set; }
}
