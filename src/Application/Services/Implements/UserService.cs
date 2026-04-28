using Mapster;
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

        public UserService(
            IEmailService emailService,
            IUserRepository userRepository,
            IConfiguration configuration,
            ITokenService tokenService)
        {
            _emailService = emailService;
            _userRepository = userRepository;
            _configuration = configuration;
            _tokenService = tokenService;
            _verificationCodeExpiry = _configuration.GetValue<int>("VerificationCode:ExpirationMinutes", 3);
        }

        public async Task<string> RegisterAsync(RegisterDTO registerDTO)
        {
            if (await _userRepository.ExistsByNameAsync(registerDTO.Name))
            {
                Log.Warning("El usuario con el nombre {Name} ya está registrado.", registerDTO.Name);
                throw new InvalidOperationException("El nombre de usuario ya está registrado.");
            }
            if (await _userRepository.ExistsByEmailAsync(registerDTO.Email))
            {
                Log.Warning("El usuario con el email {Email} ya está registrado.", registerDTO.Email);
                throw new InvalidOperationException("El email ya está registrado.");
            }
            if (await _userRepository.ExistsByRutAsync(registerDTO.Rut))
            {
                Log.Warning("El usuario con el RUT {Rut} ya está registrado.", registerDTO.Rut);
                throw new InvalidOperationException("El RUT ya está registrado.");
            }
            if (await _userRepository.ExistsByPhoneNumberAsync(registerDTO.PhoneNumber))
            {
                Log.Warning("El usuario con el teléfono {Phone} ya está registrado.", registerDTO.PhoneNumber);
                throw new InvalidOperationException("El número de teléfono ya está registrado.");
            }

            var user = registerDTO.Adapt<User>();
            await _userRepository.CreateAsync(user);

            Log.Information("Registro exitoso para el usuario {Email} con ID {Id}.", user.Email, user.Id);
            await GenerateAndSendVerificationCodeAsync(user.Id, user.Email);

            return "Se ha enviado un código de verificación a su correo. Por favor, verifica tu email.";
        }

        public async Task<string> EmailVerificationAsync(EmailVerificationDTO emailVerificationDTO)
        {
            var user = await _userRepository.GetByEmailAsync(emailVerificationDTO.Email)
                ?? throw new KeyNotFoundException("No se encontró un usuario con ese correo electrónico.");

            if (user.EmailConfirmed)
            {
                Log.Information("El correo {Email} ya fue verificado.", emailVerificationDTO.Email);
                return "El correo ya ha sido verificado.";
            }

            if (user.VerificationCodeExpiry < DateTime.UtcNow)
            {
                await GenerateAndSendVerificationCodeAsync(user.Id, user.Email);
                Log.Warning("Código expirado para {Email}. Se envió uno nuevo.", emailVerificationDTO.Email);
                throw new InvalidOperationException("El código de verificación ha expirado. Se ha enviado un nuevo código a tu correo.");
            }

            if (user.VerificationCode != emailVerificationDTO.VerificationCode)
            {
                Log.Warning("Código incorrecto para {Email}.", emailVerificationDTO.Email);
                throw new ArgumentException("El código de verificación es incorrecto.");
            }

            bool isVerified = await _userRepository.MarkEmailAsConfirmedAsync(user.Id);
            if (!isVerified)
            {
                Log.Error("Error al confirmar el correo para {Email}.", emailVerificationDTO.Email);
                throw new Exception("Error al verificar el correo electrónico. Por favor, inténtalo de nuevo.");
            }

            await _emailService.SendWelcomeEmailAsync(user.Email);
            Log.Information("Correo verificado exitosamente para {Email}.", emailVerificationDTO.Email);

            return "Correo verificado exitosamente. ¡Bienvenido a TiendaUCN!";
        }

        public async Task<string> LoginAsync(LoginDTO loginDTO)
        {
            User user = await _userRepository.GetByEmailAsync(loginDTO.Email)
                ?? throw new KeyNotFoundException("Credenciales inválidas.");

            if (user.IsDeleted)
                throw new UnauthorizedAccessException("La cuenta ha sido eliminada.");

            if (!user.EmailConfirmed)
                throw new UnauthorizedAccessException("Debes verificar tu correo electrónico antes de iniciar sesión.");

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                Log.Warning("Contraseña incorrecta para el usuario {Email}.", loginDTO.Email);
                throw new UnauthorizedAccessException("Credenciales inválidas.");
            }

            var token = _tokenService.GenerateAccessTokenAsync(user, user.Role.Name);

            Log.Information("Inicio de sesión exitoso para el usuario {Email}.", loginDTO.Email);
            return token;
        }

        public async Task<string> LogoutAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                Log.Warning("Intento de cierre de sesión con token nulo o vacío.");
                throw new ArgumentNullException("El token no puede ser nulo o vacío.");
            }
            
            await _tokenService.AddToBlacklistAsync(token);

            Log.Information("Cierre de sesión exitoso. Token añadido a la blacklist.");
            return "Sesión cerrada exitosamente.";
        }

        public async Task<string> ResendVerificationCodeAsync(ResendVerificationCodeDTO resendVerificationCodeDTO)
        {
            var user = await _userRepository.GetByEmailAsync(resendVerificationCodeDTO.Email)
                ?? throw new KeyNotFoundException("No se encontró un usuario con ese correo electrónico.");

            if (user.EmailConfirmed)
                throw new InvalidOperationException("El correo ya ha sido verificado.");

            await GenerateAndSendVerificationCodeAsync(user.Id, user.Email);

            Log.Information("Código de verificación reenviado a {Email}.", resendVerificationCodeDTO.Email);
            return "Código de verificación reenviado exitosamente.";
        }

        private async Task GenerateAndSendVerificationCodeAsync(int userId, string email)
        {
            string verificationCode = new Random().Next(100000, 999999).ToString();
            DateTime expiry = DateTime.UtcNow.AddMinutes(_verificationCodeExpiry);

            Log.Information("Generando código de verificación para {Email}.", email);

            bool isSaved = await _userRepository.SaveVerificationCodeAsync(userId, verificationCode, expiry);
            if (!isSaved)
            {
                Log.Error("Error al guardar el código de verificación para {Email}.", email);
                throw new Exception("Error al generar el código de verificación. Por favor, inténtalo de nuevo.");
            }

            await _emailService.SendVerificationCodeEmailAsync(email, verificationCode);
            Log.Information("Código de verificación enviado a {Email}.", email);
        }
    }
}