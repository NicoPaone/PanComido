using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Entidades
{
    public class BienvenidaDatosInvitadoComanda
    {
        public int IdComanda { get; set; }
        public int CantComensales { get; set; }
        public MesaConPosiciones Mesa { get; set; }
        public Restaurante RestauranteDatos { get; set; }
    }
}
