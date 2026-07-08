using System.Threading.Tasks;

namespace PanComido.Dominio.Interfaces.Servicios
{
    public interface IGeneradorNombreLoteServicio
    {
        Task<string> GenerarNombreUnicoAsync(string nombreItem);
    }
}
