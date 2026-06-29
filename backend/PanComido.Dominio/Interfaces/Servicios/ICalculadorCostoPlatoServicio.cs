using PanComido.Dominio.Entidades;
using System.Threading.Tasks;

namespace PanComido.Dominio.Interfaces.Servicios
{
    public interface ICalculadorCostoPlatoServicio
    {
        Task<decimal> CalcularCostoAsync(Plato plato);
    }
}
