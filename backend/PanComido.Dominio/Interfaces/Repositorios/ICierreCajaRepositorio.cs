using PanComido.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Interfaces.Repositorios
{
    public interface ICierreCajaRepositorio
    {
        Task<Cierre> CrearCierreDeCajaAsync(Cierre cierre);
        Task<List<Cierre>> ObtenerCierresDeCajaAsync(int restauranteId);
    }
}
