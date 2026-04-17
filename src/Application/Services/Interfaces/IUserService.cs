using TiendaUCN.src.Application.DTOs.AuthDTO;

namespace TiendaUCN.src.Domain.Models
{
    public interface IUserService
    {
        Task<string> RegisterAsync(RegisterDTO registerDTO);
        Task<string> EmailVerificationAsync(EmailVerificationDTO emailVerificationDTO);

        Task<string> LoginAsync(LoginDTO loginDTO);

        Task<string> LogoutAsync(string token);
    }
}