using System.Net;
using System.Net.Mail;
using PanComido.Dominio.Interfaces.Servicios;
using Microsoft.Extensions.Configuration;

namespace PanComido.Infraestructura.ServiciosExternos
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task EnviarEmailAsync(string emailDestino, string asunto, string cuerpo)
        {
            var emailOrigen = _configuration["Smtp:Email"] ?? "pancomido.unlam@gmail.com";
            var passwordOrigen = _configuration["Smtp:Password"] ?? "jqci xwmo uqgq kgii";

            var fromAddress = new MailAddress(emailOrigen, "Pan Comido");
            var toAddress = new MailAddress(emailDestino);
            var fromPassword = passwordOrigen;

            using var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = asunto,
                Body = cuerpo,
                IsBodyHtml = true
            };

            await smtp.SendMailAsync(message);
        }
    }
}
