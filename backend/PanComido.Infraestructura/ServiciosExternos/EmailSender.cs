using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using PanComido.Dominio.Interfaces.Servicios;

namespace PanComido.Infraestructura.ServiciosExternos
{
    public class EmailSender : IEmailSender
    {
        public async Task EnviarEmailAsync(string emailDestino, string asunto, string cuerpo)
        {
            var fromAddress = new MailAddress("pancomido.unlam@gmail.com", "Pan Comido");
            var toAddress = new MailAddress(emailDestino);
            const string fromPassword = "jqci xwmo uqgq kgii";

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
