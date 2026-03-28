using Microsoft.AspNetCore.Mvc;
using VanSalesJourneyPlan.API.DTOs;
using VanSalesJourneyPlan.API.Services;

namespace VanSalesJourneyPlan.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, IJwtTokenService jwtTokenService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    /// <summary>
    /// Login with username and password to get JWT token
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse 
            { 
                Success = false, 
                Message = "Invalid request" 
            });
        }

        var (success, message, user) = await _authService.LoginAsync(request.Username, request.Password);

        if (!success || user == null)
        {
            _logger.LogWarning($"Failed login attempt for username: {request.Username}");
            return Unauthorized(new ApiResponse 
            { 
                Success = false, 
                Message = message 
            });
        }

        var token = _jwtTokenService.GenerateToken(user);
        _logger.LogInformation($"User {user.Username} logged in successfully");

        return Ok(new LoginResponseDto
        {
            Success = true,
            Message = "Login successful",
            Token = token,
            User = new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role
            }
        });
    }

    /// <summary>
    /// Logout endpoint (placeholder - real logout is handled client-side by token deletion)
    /// </summary>
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok(new ApiResponse 
        { 
            Success = true, 
            Message = "Logout successful. Please discard your token." 
        });
    }
}
