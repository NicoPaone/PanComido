using System;
using System.Collections.Generic;

namespace PanComido.Dominio.Entidades
{
    public class MozoMesa
    {
        public int MesaId { get; set; }
        public Mesa Mesa { get; set; }

        public int MozoId { get; set; }
        // public Mozo Mozo { get; set; }
    }
}
