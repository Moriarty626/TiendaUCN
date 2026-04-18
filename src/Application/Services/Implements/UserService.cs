using Mapster;
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
            // Verificar si el email ya está registrado
            bool isRegisteredByEmail = await _userRepository.ExistsByEmailAsync(registerDTO.Email);
            if (isRegisteredByEmail)
            {
                Log.Warning($"El usuario con el email {registerDTO.Email} ya está registrado.");
                throw new InvalidOperationException("El email ya está registrado.");
            }
            // Verificar si el RUT ya está registrado
            bool isRegisteredByRut = await _userRepository.ExistsByRutAsync(registerDTO.Rut);
            if (isRegisteredByRut)
            {
                Log.Warning($"El usuario con el RUT {registerDTO.Rut} ya está registrado.");
                throw new InvalidOperationException("El RUT ya está registrado.");
            }

            // Verificar si el número de teléfono ya está registrado
            bool isRegisteredByPhoneNumber = await _userRepository.ExistsByPhoneNumberAsync(registerDTO.PhoneNumber);
            if (isRegisteredByPhoneNumber)
            {
                Log.Warning($"El usuario con el número de teléfono {registerDTO.PhoneNumber} ya está registrado.");
                throw new InvalidOperationException("El número de teléfono ya está registrado.");
            }

            // Crear el nuevo usuario
            var user = registerDTO.Adapt<User>();
            await _userRepository.CreateAsync(user);

            Log.Information($"Registro exitoso para el usuario {user.Email} con ID {user.Id}.");
            // Generar y enviar el código de verificación
            await GenerateAndSendVerificationCodeAsync(user.Id, user.Email);

            // Retornar un mensaje de éxito o un token de verificación
            return $"Se ha envidado un código de verificación a su correo. Por favor, verifica tu email antes que expi";


        }

        private async Task GenerateAndSendVerificationCodeAsync(int userId, string email)
        {
            string verificationCode = new Random().Next(100000, 999999).ToString();
            DateTime verificationCodeExpiry = DateTime.UtcNow.AddMinutes(_verificationCodeExpiry);
            Log.Information($"Generando código de verificación para el usuario: {email} - Código: {verificationCode}.");

            bool isSaved = await _userRepository.SaveVerificationCodeAsync(userId, verificationCode, verificationCodeExpiry);

            if (!isSaved)
            {
                Log.Error($"Error al guardar el código de verificación para el usuario: {email}.");
                throw new Exception("Error al generar el código de verificación. Por favor, inténtalo de nuevo.");
            }

            await _emailService.SendVerificationCodeEmailAsync(email, verificationCode);
            Log.Information($"Código de verificación enviado al correo: {email}.");
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

        public Task<string> ResendVerificationCodeAsync(ResendVerificationCodeDTO resendVerificationCodeDTO)
        {
            throw new NotImplementedException();
        }
    }
}