using Authentication.Api.Model.Dto;
using Authentication.Api.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IUserService _userService;
        private ILogger<AuthController> _logger;

        public AuthController(
            IUserService userService,
            ILogger<AuthController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // /api/v1/Auth/register
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.RegisterUserAsync(request);

                if (result.IsSuccess)
                    return Ok(result);

                return BadRequest(result);
            }
            _logger.LogWarning("User could not be registered because request is invalid.");
            return BadRequest("Request is invalid.");
        }

        // /api/v1/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.AuthenticateUserAsync(request);

                if (result.IsSuccess)
                    return Ok(result);

                return BadRequest(result);
            }

            return BadRequest("Something went wrong.");
        }
    }
}
