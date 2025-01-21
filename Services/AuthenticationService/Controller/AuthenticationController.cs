using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace YawShop.Services.AuthenticationService.Controller;

/// <summary>
/// Used for login. Apiendpointidentity requires authenticated user.
/// TODO: 2fa is behind auth now+
/// </summary>
[ApiController]
[Route("/api/v1/auth/public")]
public class AuthenticationController : ControllerBase
{
    private readonly ILogger<AuthenticationController> _logger;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AuthenticationController(
       ILogger<AuthenticationController> logger,
       UserManager<IdentityUser> userManager,
       SignInManager<IdentityUser> signInManager)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [AllowAnonymous]
    [HttpPost("check")]
    public IActionResult CheckAuthenticated([FromBody] object empty)
    {
        if (empty != null)
        {
            return Ok();
        }
        return Unauthorized();
    }

    // Public login endpoint
    [HttpPost("login")]
    [AllowAnonymous] // No authentication required for login
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: true, lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            return Unauthorized(new { Message = "Invalid credentials." });
        }

        return Ok(new { Message = "Login successful." });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] object empty)
    {
        if (empty != null)
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }
        return Unauthorized();
    }

}