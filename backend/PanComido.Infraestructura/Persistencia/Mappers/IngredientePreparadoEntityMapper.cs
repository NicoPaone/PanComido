using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia.Mappers
{
    public class IngredientePreparadoEntityMapper
    {
        public DOM.IngredientePreparado ParaDominio(EF.IngredientePreparado efIngredientePreparado)
        {
            
            var ingrediente = efIngredientePreparado.IdIngredienteNavigation;
            var insumo = ingrediente?.IdInsumoNavigation;
            var articulo = insumo?.IdArticuloNavigation;
            var unidadMedida = insumo?.UnidadMedida;

           
            return new DOM.IngredientePreparado
            {
                
                Id = efIngredientePreparado.IdIngrediente,

               
                Nombre = articulo?.Nombre ?? "Sin nombre",
                StockMinimo = insumo?.StockMinimo ?? 0,
                StockRecomendado = insumo?.StockRecomendado ?? 0,


                unidadMedida = unidadMedida?.Nombre ?? ""
            };
        }
    }
}