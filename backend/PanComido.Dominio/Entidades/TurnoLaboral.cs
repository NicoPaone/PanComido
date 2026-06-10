using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Entidades
{
    public class TurnoLaboral
    {
        public int Id { get; set; }
        public int RestauranteId { get; set; }
        public TimeOnly HorarioInicio { get; set; }
        public TimeOnly HorarioFin {  get; set; }
        public bool EsNocturno { get; set; }
    }
}
