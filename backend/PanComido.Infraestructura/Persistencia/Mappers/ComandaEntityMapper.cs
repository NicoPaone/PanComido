using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Infraestructura.Persistencia.Entidades;
using System.Collections.Generic;
using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia.Mappers
{
    public class ComandaEntityMapper
    {
        public DOM.Comanda ParaDominio(EF.Comandum efComanda)
        {
            if (efComanda == null) return null;

            var comandaDominio = new DOM.Comanda
            {
                Id = efComanda.Id,
                MesaId = efComanda.MesaId,
                RestauranteId = efComanda.RestauranteId,
                PagoID = efComanda.PagoId,
                CantComensales = efComanda.CantComensales,
                HoraInicio = efComanda.HoraInicio,
                HoraFin = efComanda.HoraFin,
                Estado = (EstadoComanda)efComanda.EstadoComandaId,

                Platos = new List<DOM.Plato>()
            };

          
            foreach (var relacion in efComanda.ArticuloComanda)
            {
                if (relacion.Articulo != null && relacion.Articulo.Plato != null)
                {
                   
                    comandaDominio.Platos.Add(new DOM.Plato
                    {
                        Id = relacion.Articulo.Id,
                        Nombre = relacion.Articulo.Nombre,
                        TiempoPreparacionBase = relacion.Articulo.Plato.TiempoPreparacionBase,
                       
                        Cantidad = relacion.Cantidad,
                        ObservacionesGenerales = relacion.ObservacionesGenerales,
                        ObservacionesIngredientes = relacion.ObservacionesIngrediente,
                        Entregado = relacion.Entregado

                     


                    });
                }
            }

        
            return comandaDominio;
        }

        public Comandum paraEntidad(DOM.Comanda comandaDominio)
        {
            return new EF.Comandum
            {
                Id = comandaDominio.Id,
                MesaId = comandaDominio.MesaId,
                RestauranteId = comandaDominio.RestauranteId,
                EstadoComandaId = (int)comandaDominio.Estado,
                CantComensales = comandaDominio.CantComensales,
                HoraInicio = comandaDominio.HoraInicio,
                HoraFin = comandaDominio.HoraFin,
                PagoId = comandaDominio.PagoID,
            };
        }
    }
}