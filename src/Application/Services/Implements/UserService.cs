using Resend;
using Serilog;
using TiendaUCN.src.Application.DTOs.AuthDTO;
using TiendaUCN.src.Application.Services.Interfaces;

namespace TiendaUCN.src.Domain.Models

{
    public class UserService : IUserService
    {
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;

        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        private readonly int _verificationCodeExpiry;

        public UserService(IEmailService emailService, IUserRepository userRepository, IConfiguration configuration, ITokenService tokenService)
        {
            _emailService = emailService;
            _userRepository = userRepository;
            _configuration = configuration;
            _tokenService = tokenService;
            _verificationCodeExpiry = _configuration.GetValue<int>("VerificationCode:ExpirationTimeInMinutes");
        }
        public async Task<string> RegisterAsync(RegisterDTO registerDTO)
        {
            // Verificar si el usuario ya existe
            bool isRegisteredByName = await _userRepository.ExistsByNameAsync(registerDTO.Name);
            if (isRegisteredByName)
            {
                Log.Warning($"El usuario con el nombre {registerDTO.Name} ya está registrado.");
                throw new InvalidOperationException("El nombre de usuario ya está registrado.");
            }

            bool isRegisteredByEmail = await _userRepository.ExistsByEmailAsync(registerDTO.Email);
            if (isRegisteredByEmail)
            {
                Log.Warning($"El usuario con el email {registerDTO.Email} ya está registrado.");
                throw new InvalidOperationException("El email ya está registrado.");
            }

            bool isRegisteredByRut = await _userRepository.ExistsByRutAsync(registerDTO.Rut);
            if (isRegisteredByRut)
            {
                Log.Warning($"El usuario con el RUT {registerDTO.Rut} ya está registrado.");
                throw new InvalidOperationException("El RUT ya está registrado.");
            }

            bool isRegisteredByPhoneNumber = await _userRepository.ExistsByPhoneNumberAsync(registerDTO.PhoneNumber);
            if (isRegisteredByPhoneNumber)
            {
                Log.Warning($"El usuario con el número de teléfono {registerDTO.PhoneNumber} ya está registrado.");
                throw new InvalidOperationException("El número de teléfono ya está registrado.");
            }

            // Implementar lógica de inicio de sesión
            throw new NotImplementedException();

        }

        public async Task<string> EmailVerificationAsync(EmailVerificationDTO emailVerificationDTO)
        {
            // Implementar lógica de inicio de sesión
            throw new NotImplementedException();
        }

        public async Task<string> LoginAsync(LoginDTO loginDTO)
        {
            // Implementar lógica de inicio de sesión
            throw new NotImplementedException();
        }
        public async Task<string> LogoutAsync(string token)
        {
            // Implementar lógica de cierre de sesión
            throw new NotImplementedException();
        }


    }
}