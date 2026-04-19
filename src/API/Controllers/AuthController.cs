using Microsoft.AspNetCore.Mvc;
using TiendaUCN.src.Application.DTOs.AuthDTO;
using TiendaUCN.src.Application.Services.Interfaces;
using TiendaUCN.src.Domain.Models;

namespace TiendaUCN.src.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IUserService userService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            var result = await userService.RegisterAsync(registerDTO);
            return Ok(new { message = result });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var token = await userService.LoginAsync(loginDTO);
            return Ok(new { token });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await userService.LogoutAsync(token);
            return Ok(new { message = result });
        }

        [HttpPost("email-verification")]
        public async Task<IActionResult> VerifyEmail(EmailVerificationDTO emailVerificationDTO)
        {
            var result = await userService.EmailVerificationAsync(emailVerificationDTO);
            return Ok(new { message = result });
        }
    }
}