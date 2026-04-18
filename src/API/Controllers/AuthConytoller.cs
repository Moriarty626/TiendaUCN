using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaUCN.src.Application.DTOs.AuthDTO;
using TiendaUCN.src.Application.DTOs.BaseResponse;
using TiendaUCN.src.Domain.Models;

namespace TiendaUCN.src.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //http://localhost:5000/api/auth/register
    public class AuthConytoller : ControllerBase
    {
        private readonly IUserService _userService;
        public AuthConytoller(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            var menssage = await _userService.RegisterAsync(registerDTO);
            return Ok(new GenericResponse<string>("Registro exitoso", menssage));
        }

        [HttpPost("email-verification")]
        public async Task<IActionResult> EmailVerification([FromBody] EmailVerificationDTO emailVerificationDTO)
        {
            await _userService.EmailVerificationAsync(emailVerificationDTO);
            return Ok(new GenericResponse<string>("Verificación exitosa", null));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var token = await _userService.LoginAsync(loginDTO);
            return Ok(new GenericResponse<string>("Inicio de sesión exitoso", token));
        }

        [HttpPost("resend-verification-code")]
        public async Task<IActionResult> ResendVerificationCode([FromBody] ResendVerificationCodeDTO resendVerificationCodeDTO)
        {
            var message = await _userService.ResendVerificationCodeAsync(resendVerificationCodeDTO);
            return Ok(new GenericResponse<string>("Código de verificación reenviado exitosamente", message));
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
            var message = await _userService.LogoutAsync(token);
            return Ok(new GenericResponse<string>(message, null));
        }

    }
}