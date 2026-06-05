using PanComido.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Interfaces.Servicios
{
    public interface IComandaNotificador
    {
        Task NotificarEstadoModificadoAsync(Comanda comanda, List<int> mozoIds);
<<<<<<< HEAD
=======

>>>>>>> b32a5e1 (cambios no actualizados)
        Task NotificarLlamadoCocinaAsync(Comanda comanda, List<int> mozoIds);
    }
}
