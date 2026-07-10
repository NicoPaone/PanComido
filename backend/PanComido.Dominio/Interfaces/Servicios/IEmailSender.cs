using System.Threading.Tasks;

namespace PanComido.Dominio.Interfaces.Servicios
{
    public interface IEmailSender
    {
        Task EnviarEmailAsync(string emailDestino, string asunto, string cuerpo);
    }
}
