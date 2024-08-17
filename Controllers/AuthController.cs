using Microsoft.AspNetCore.Mvc;
using WebAPP.Services;

namespace WebAPP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                var user = await _userService.RegisterAsync(model.Username, model.Password, model.Email);
                return Ok(new { user.Username, user.Email });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userService.AuthenticateAsync(model.Username, model.Password);
            if (user == null)
                return Unauthorized(new { message = "Invalid credentials" });

            var token = _userService.GenerateJwtToken(user);
            var refreshToken = await _userService.GenerateRefreshTokenAsync(user.ID);

            return Ok(new { accessToken = token, refreshToken = refreshToken.Token });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenModel model)
        {
            var refreshToken = await _userService.GetRefreshTokenAsync(model.RefreshToken);
            if (refreshToken == null)
                return Unauthorized(new { message = "Invalid refresh token" });

            var newJwtToken = _userService.GenerateJwtToken(refreshToken.User);
            var newRefreshToken = await _userService.GenerateRefreshTokenAsync(refreshToken.UserId);


            return Ok(new { accessToken = newJwtToken, refreshToken = newRefreshToken.Token });
        }
    }

    public class RegisterModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class LoginModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RefreshTokenModel
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
