using Resend;
using TiendaUCN.src.Application.Services.Interfaces;

namespace TiendaUCN.src.Application.Services.Implements
{
    public class EmailService : IEmailService
    {
        private readonly IResend _resend;
        private readonly IConfiguration _configuration;

        public EmailService(IResend resend, IConfiguration configuration)
        {
            _resend = resend;
            _configuration = configuration;
        }

        public async Task SendVerificationCodeEmailAsync(string email, string verificationCode)
        {
            var message = new EmailMessage
            {
                To = email,
                Subject = _configuration["EmailSettings:VerificationCodeSubject"] ?? throw new ArgumentNullException("El correo verificado no es valido"),
                From = _configuration["EmailConfiguration:From"] ?? throw new ArgumentNullException("El correo de origen no está configurado."),
                HtmlBody = $"<p style= 'font-size: 48px; font-weight: bold; letter-spacing: 8px;'>{verificationCode}</p>"
            };

            await _resend.EmailSendAsync(message);
        }


        public async Task SendWelcomeEmailAsync(string email)
        {
            var message = new EmailMessage
            {
                To = email,
                Subject = _configuration["EmailSettings:WelcomeSubject"] ?? throw new ArgumentNullException("El asunto del correo de bienvenida"),
                From = _configuration["EmailConfiguration:From"] ?? throw new ArgumentNullException("El correo de origen no está configurado."),
                HtmlBody = $"<p style='font-size: 24px; font-weight: bold;'> Tu cuenta ha sido Verificada</p>"
            };
            await _resend.EmailSendAsync(message);
        }
    }
}