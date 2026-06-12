using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Interfaces.Servicios
{
    public interface ICantidadDeMesasServicio
    {
        public int ObtenerCantidadDeMesasTotal(int restauranteId);
        public int ObtenerCantidadDeMesasOcupadas(int restauranteId);
        public int ObtenerCantidadDeMesasDisponibles(int restauranteId);
    }
}
