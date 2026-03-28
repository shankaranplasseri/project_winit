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
public class JourneyPlansController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<JourneyPlansController> _logger;

    public JourneyPlansController(AppDbContext context, ILogger<JourneyPlansController> logger)
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
    /// Get my assigned journey plans (current user) or all plans (Admin)
    /// </summary>
    [HttpGet("my")]
    public async Task<IActionResult> GetMyJourneyPlans()
    {
        try
        {
            int userId = GetCurrentUserId();
            string role = GetCurrentUserRole();

            IQueryable<JourneyPlan> query = _context.JourneyPlans
                .Include(jp => jp.AssignedUser)
                .Include(jp => jp.CreatedUser)
                .Include(jp => jp.JourneyPlanItems)
                    .ThenInclude(jpi => jpi.Customer);

            if (role != "Admin")
            {
                query = query.Where(jp => jp.AssignedUserId == userId);
            }

            var plans = await query.OrderByDescending(jp => jp.PlanDate).ToListAsync();

            var dtos = plans.Select(p => new JourneyPlanDto
            {
                JourneyPlanId = p.JourneyPlanId,
                AssignedUserId = p.AssignedUserId,
                AssignedUserName = p.AssignedUser?.Username,
                PlanDate = p.PlanDate.ToString("yyyy-MM-dd"),
                Title = p.Title,
                Description = p.Description,
                Status = p.Status,
                Items = p.JourneyPlanItems.Select(jpi => new JourneyPlanItemDto
                {
                    JourneyPlanItemId = jpi.JourneyPlanItemId,
                    CustomerId = jpi.CustomerId,
                    CustomerCode = jpi.Customer?.CustomerCode,
                    CustomerName = jpi.Customer?.CustomerName,
                    SequenceNumber = jpi.SequenceNumber,
                    Notes = jpi.Notes,
                    PlannedVisitTime = jpi.PlannedVisitTime?.ToString("HH:mm"),
                    IsCompleted = jpi.IsCompleted
                }).ToList()
            }).ToList();

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Journey plans retrieved successfully",
                Data = dtos
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving journey plans");
            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = $"Error retrieving journey plans: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Get journey plan by ID (user must be assigned or admin)
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetJourneyPlanById(int id)
    {
        try
        {
            int userId = GetCurrentUserId();
            string role = GetCurrentUserRole();

            var plan = await _context.JourneyPlans
                .Include(jp => jp.AssignedUser)
                .Include(jp => jp.CreatedUser)
                .Include(jp => jp.JourneyPlanItems)
                    .ThenInclude(jpi => jpi.Customer)
                .FirstOrDefaultAsync(jp => jp.JourneyPlanId == id);

            if (plan == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Journey plan not found"
                });
            }

            // Check authorization
            if (role != "Admin" && plan.AssignedUserId != userId)
            {
                return Forbid();
            }

            var dto = new JourneyPlanDto
            {
                JourneyPlanId = plan.JourneyPlanId,
                AssignedUserId = plan.AssignedUserId,
                AssignedUserName = plan.AssignedUser?.Username,
                PlanDate = plan.PlanDate.ToString("yyyy-MM-dd"),
                Title = plan.Title,
                Description = plan.Description,
                Status = plan.Status,
                Items = plan.JourneyPlanItems.Select(jpi => new JourneyPlanItemDto
                {
                    JourneyPlanItemId = jpi.JourneyPlanItemId,
                    CustomerId = jpi.CustomerId,
                    CustomerCode = jpi.Customer?.CustomerCode,
                    CustomerName = jpi.Customer?.CustomerName,
                    SequenceNumber = jpi.SequenceNumber,
                    Notes = jpi.Notes,
                    PlannedVisitTime = jpi.PlannedVisitTime?.ToString("HH:mm"),
                    IsCompleted = jpi.IsCompleted
                }).ToList()
            };

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Journey plan retrieved successfully",
                Data = dto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving journey plan");
            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = $"Error retrieving journey plan: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Create a new journey plan (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateJourneyPlan([FromBody] CreateJourneyPlanRequest request)
    {
        try
        {
            if (!ModelState.IsValid || !DateOnly.TryParse(request.PlanDate, out var planDate))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid request"
                });
            }

            // Verify assigned user exists
            var user = await _context.Users.FindAsync(request.AssignedUserId);
            if (user == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Assigned user not found"
                });
            }

            int currentUserId = GetCurrentUserId();

            var plan = new JourneyPlan
            {
                AssignedUserId = request.AssignedUserId,
                PlanDate = planDate,
                Title = request.Title,
                Description = request.Description,
                Status = "Draft",
                CreatedUserId = currentUserId
            };

            _context.JourneyPlans.Add(plan);
            await _context.SaveChangesAsync();

            // Add journey plan items
            int sequence = 1;
            foreach (var customerId in request.CustomerIds)
            {
                var customer = await _context.Customers.FindAsync(customerId);
                if (customer != null)
                {
                    var item = new JourneyPlanItem
                    {
                        JourneyPlanId = plan.JourneyPlanId,
                        CustomerId = customerId,
                        SequenceNumber = sequence++
                    };
                    _context.JourneyPlanItems.Add(item);
                }
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Journey plan {plan.JourneyPlanId} created for user {request.AssignedUserId}");

            return Created($"api/journeyplans/{plan.JourneyPlanId}", new ApiResponse
            {
                Success = true,
                Message = "Journey plan created successfully",
                Data = plan.JourneyPlanId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating journey plan");
            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = $"Error creating journey plan: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Update a journey plan (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateJourneyPlan(int id, [FromBody] UpdateJourneyPlanRequest request)
    {
        try
        {
            var plan = await _context.JourneyPlans.FindAsync(id);
            if (plan == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Journey plan not found"
                });
            }

            if (!string.IsNullOrEmpty(request.Title))
                plan.Title = request.Title;
            if (!string.IsNullOrEmpty(request.Description))
                plan.Description = request.Description;
            if (!string.IsNullOrEmpty(request.Status))
                plan.Status = request.Status;

            plan.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Journey plan {id} updated successfully");

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Journey plan updated successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating journey plan");
            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = $"Error updating journey plan: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Delete a journey plan (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteJourneyPlan(int id)
    {
        try
        {
            var plan = await _context.JourneyPlans
                .Include(jp => jp.JourneyPlanItems)
                .FirstOrDefaultAsync(jp => jp.JourneyPlanId == id);

            if (plan == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Journey plan not found"
                });
            }

            _context.JourneyPlans.Remove(plan);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Journey plan {id} deleted successfully");

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Journey plan deleted successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting journey plan");
            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = $"Error deleting journey plan: {ex.Message}"
            });
        }
    }
}
