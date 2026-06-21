using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia.Mappers
{
    public class LlamadoEntityMapper
    {
        public DOM.Llamado paraDominio(EF.Llamado efLlamado)
        {
            return new DOM.Llamado
            {
                Id = efLlamado.Id,
                MozoId = efLlamado.MozoId,
                MesaId = efLlamado.MesaId,
                NumeroDeMesa = efLlamado.Mesa.Numero,
                GerenteId = efLlamado.GerenteId,
                CategoriaLlamadoId = efLlamado.CategoriaLlamadoId,
                CategoriaDescripcion = efLlamado.CategoriaLlamado.Descripcion,
                Descripcion = efLlamado.Descripcion,
                Resuelto = efLlamado.Resuelto
            };
        }

        public EF.Llamado paraEntidad(DOM.Llamado llamado)
        {
            return new EF.Llamado
            {
                MozoId = llamado.MozoId,
                MesaId = llamado.MesaId,
                GerenteId = llamado.GerenteId,
                CategoriaLlamadoId = llamado.CategoriaLlamadoId,
                Descripcion = llamado.Descripcion,
                Resuelto = llamado.Resuelto
            };
        }
    }
}
