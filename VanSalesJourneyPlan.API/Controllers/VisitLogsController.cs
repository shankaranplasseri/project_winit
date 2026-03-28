using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VanSalesJourneyPlan.API.Data;
using VanSalesJourneyPlan.API.DTOs;
using VanSalesJourneyPlan.API.Models;

namespace VanSalesJourneyPlan.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VisitLogsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<VisitLogsController> _logger;

    public VisitLogsController(AppDbContext context, ILogger<VisitLogsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
    }

    private string GetCurrentUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
    }

    /// <summary>
    /// Get visit logs for a journey plan item
    /// </summary>
    [HttpGet("byitem/{journeyPlanItemId}")]
    public async Task<IActionResult> GetVisitLogsByJourneyPlanItem(int journeyPlanItemId)
    {
        try
        {
            int userId = GetCurrentUserId();
            string role = GetCurrentUserRole();

            // Verify the user has access to this plan item
            var planItem = await _context.JourneyPlanItems
                .Include(jpi => jpi.JourneyPlan)
                .FirstOrDefaultAsync(jpi => jpi.JourneyPlanItemId == journeyPlanItemId);

            if (planItem == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Journey plan item not found"
                });
            }

            if (role != "Admin" && planItem.JourneyPlan!.AssignedUserId != userId)
            {
                return Forbid();
            }

            var logs = await _context.VisitLogs
                .Where(vl => vl.JourneyPlanItemId == journeyPlanItemId)
                .OrderByDescending(vl => vl.VisitDate)
                .ThenByDescending(vl => vl.VisitTime)
                .ToListAsync();

            var dtos = logs.Select(log => new VisitLogDto
            {
                VisitLogId = log.VisitLogId,
                JourneyPlanItemId = log.JourneyPlanItemId,
                VisitDate = log.VisitDate.ToString("yyyy-MM-dd"),
                VisitTime = log.VisitTime?.ToString("HH:mm"),
                Notes = log.Notes,
                SalesAmount = log.SalesAmount,
                Status = log.Status
            }).ToList();

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Visit logs retrieved successfully",
                Data = dtos
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving visit logs");
            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = $"Error retrieving visit logs: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Create a new visit log
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateVisitLog([FromBody] CreateVisitLogRequest request)
    {
        try
        {
            if (!ModelState.IsValid || !DateOnly.TryParse(request.VisitDate, out var visitDate))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid request"
                });
            }

            int userId = GetCurrentUserId();
            string role = GetCurrentUserRole();

            // Verify the journey plan item exists and user has access
            var planItem = await _context.JourneyPlanItems
                .Include(jpi => jpi.JourneyPlan)
                .FirstOrDefaultAsync(jpi => jpi.JourneyPlanItemId == request.JourneyPlanItemId);

            if (planItem == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Journey plan item not found"
                });
            }

            if (role != "Admin" && planItem.JourneyPlan!.AssignedUserId != userId)
            {
                return Forbid();
            }

            TimeOnly? visitTime = null;
            if (!string.IsNullOrEmpty(request.VisitTime))
            {
                if (TimeOnly.TryParse(request.VisitTime, out var time))
                {
                    visitTime = time;
                }
            }

            var log = new VisitLog
            {
                JourneyPlanItemId = request.JourneyPlanItemId,
                VisitDate = visitDate,
                VisitTime = visitTime,
                Notes = request.Notes,
                SalesAmount = request.SalesAmount,
                Status = "Completed"
            };

            _context.VisitLogs.Add(log);

            // Mark the journey plan item as completed if first visit
            planItem.IsCompleted = true;
            planItem.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Visit log {log.VisitLogId} created for plan item {request.JourneyPlanItemId}");

            return Created($"api/visitlogs/{log.VisitLogId}", new ApiResponse
            {
                Success = true,
                Message = "Visit log created successfully",
                Data = log.VisitLogId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating visit log");
            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = $"Error creating visit log: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Update a visit log
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVisitLog(int id, [FromBody] UpdateVisitLogRequest request)
    {
        try
        {
            int userId = GetCurrentUserId();
            string role = GetCurrentUserRole();

            var log = await _context.VisitLogs
                .Include(vl => vl.JourneyPlanItem)
                    .ThenInclude(jpi => jpi!.JourneyPlan)
                .FirstOrDefaultAsync(vl => vl.VisitLogId == id);

            if (log == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Visit log not found"
                });
            }

            // Check authorization
            if (role != "Admin" && log.JourneyPlanItem?.JourneyPlan?.AssignedUserId != userId)
            {
                return Forbid();
            }

            if (!string.IsNullOrEmpty(request.VisitTime))
            {
                if (TimeOnly.TryParse(request.VisitTime, out var time))
                {
                    log.VisitTime = time;
                }
            }

            if (!string.IsNullOrEmpty(request.Notes))
                log.Notes = request.Notes;
            if (request.SalesAmount.HasValue)
                log.SalesAmount = request.SalesAmount;
            if (!string.IsNullOrEmpty(request.Status))
                log.Status = request.Status;

            log.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Visit log {id} updated successfully");

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Visit log updated successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating visit log");
            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = $"Error updating visit log: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Delete a visit log
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVisitLog(int id)
    {
        try
        {
            int userId = GetCurrentUserId();
            string role = GetCurrentUserRole();

            var log = await _context.VisitLogs
                .Include(vl => vl.JourneyPlanItem)
                    .ThenInclude(jpi => jpi!.JourneyPlan)
                .FirstOrDefaultAsync(vl => vl.VisitLogId == id);

            if (log == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Visit log not found"
                });
            }

            // Check authorization
            if (role != "Admin" && log.JourneyPlanItem?.JourneyPlan?.AssignedUserId != userId)
            {
                return Forbid();
            }

            _context.VisitLogs.Remove(log);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Visit log {id} deleted successfully");

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Visit log deleted successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting visit log");
            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = $"Error deleting visit log: {ex.Message}"
            });
        }
    }
}
