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
public class CustomersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(AppDbContext context, ILogger<CustomersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all customers (Admin only)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetCustomers()
    {
        try
        {
            var customers = await _context.Customers.ToListAsync();
            var dtos = customers.Select(c => new CustomerDto
            {
                CustomerId = c.CustomerId,
                CustomerCode = c.CustomerCode,
                CustomerName = c.CustomerName,
                Address = c.Address,
                City = c.City,
                PostalCode = c.PostalCode,
                ContactNumber = c.ContactNumber,
                Email = c.Email,
                LocationLatitude = c.LocationLatitude,
                LocationLongitude = c.LocationLongitude,
                Route = c.Route,
                Status = c.Status
            }).ToList();

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Customers retrieved successfully",
                Data = dtos
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customers");
            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = $"Error retrieving customers: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Get customer by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCustomerById(int id)
    {
        try
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Customer not found"
                });
            }

            var dto = new CustomerDto
            {
                CustomerId = customer.CustomerId,
                CustomerCode = customer.CustomerCode,
                CustomerName = customer.CustomerName,
                Address = customer.Address,
                City = customer.City,
                PostalCode = customer.PostalCode,
                ContactNumber = customer.ContactNumber,
                Email = customer.Email,
                LocationLatitude = customer.LocationLatitude,
                LocationLongitude = customer.LocationLongitude,
                Route = customer.Route,
                Status = customer.Status
            };

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Customer retrieved successfully",
                Data = dto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer");
            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = $"Error retrieving customer: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Create a new customer (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid request"
                });
            }

            // Check if customer code already exists
            var existingCustomer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerCode == request.CustomerCode);
            if (existingCustomer != null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Customer code already exists"
                });
            }

            var customer = new Customer
            {
                CustomerCode = request.CustomerCode,
                CustomerName = request.CustomerName,
                Address = request.Address,
                City = request.City,
                PostalCode = request.PostalCode,
                ContactNumber = request.ContactNumber,
                Email = request.Email,
                LocationLatitude = request.LocationLatitude,
                LocationLongitude = request.LocationLongitude,
                Route = request.Route,
                Status = "Active"
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Customer {customer.CustomerCode} created successfully");

            return Created($"api/customers/{customer.CustomerId}", new ApiResponse
            {
                Success = true,
                Message = "Customer created successfully",
                Data = customer.CustomerId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating customer");
            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = $"Error creating customer: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Update a customer (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateCustomer(int id, [FromBody] UpdateCustomerRequest request)
    {
        try
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Customer not found"
                });
            }

            if (!string.IsNullOrEmpty(request.CustomerName))
                customer.CustomerName = request.CustomerName;
            if (!string.IsNullOrEmpty(request.Address))
                customer.Address = request.Address;
            if (!string.IsNullOrEmpty(request.City))
                customer.City = request.City;
            if (!string.IsNullOrEmpty(request.PostalCode))
                customer.PostalCode = request.PostalCode;
            if (!string.IsNullOrEmpty(request.ContactNumber))
                customer.ContactNumber = request.ContactNumber;
            if (!string.IsNullOrEmpty(request.Email))
                customer.Email = request.Email;
            if (request.LocationLatitude.HasValue)
                customer.LocationLatitude = request.LocationLatitude;
            if (request.LocationLongitude.HasValue)
                customer.LocationLongitude = request.LocationLongitude;
            if (!string.IsNullOrEmpty(request.Route))
                customer.Route = request.Route;
            if (!string.IsNullOrEmpty(request.Status))
                customer.Status = request.Status;

            customer.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Customer {customer.CustomerCode} updated successfully");

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Customer updated successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating customer");
            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = $"Error updating customer: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Delete a customer (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        try
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "Customer not found"
                });
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Customer {customer.CustomerCode} deleted successfully");

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Customer deleted successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting customer");
            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Message = $"Error deleting customer: {ex.Message}"
            });
        }
    }
}
