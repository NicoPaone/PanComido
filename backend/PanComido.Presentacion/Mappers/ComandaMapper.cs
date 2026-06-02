using PanComido.Dominio.Entidades;
using PanComido.Presentacion.DTOs;
using PanComido.Presentacion.DTOs.Articulo;
using DOM = PanComido.Dominio.Entidades;

namespace PanComido.Presentacion.Mappers
{
    public class ComandaMapper
    {
        //Para una sola comanda 
        public ComandaResponseDto ComandaResponseDto(DOM.Comanda comanda)
        {
            return new ComandaResponseDto
            {
                Id = comanda.Id,
                MesaId = comanda.MesaId,
                CantComensales = comanda.CantComensales,
                Estado = comanda.Estado.ToString(),
                HoraInicio = comanda.HoraInicio.ToString("dd/MM/yyyy HH:mm"),
                HoraFin = comanda.HoraFin?.ToString("dd/MM/yyyy HH:mm"),

                TiempoEstimadoTotal = comanda.Items
                    .Select(i => i.Articulo)
                    .OfType<Plato>()
                    .Select(plato => plato.TiempoPreparacionBase)
                    .DefaultIfEmpty(0)
                    .Max(),

                Items = comanda.Items.Select(ac => new ArticuloComandaResponseDto
                {
                    Id = ac.Id,
                    Entregado = ac.Entregado,
                    Cantidad = ac.Cantidad,
                    ObservacionesGenerales = ac.ObservacionesGenerales,
                    ObservacionesIngredientes = ac.ObservacionesIngredientes,
                    Articulo = new ArticuloResponseDto
                    {
                        Id = ac.Articulo.Id,
                        Nombre = ac.Articulo.Nombre,
                    }
                }).ToList()
            };
        }

        public List<ComandaResponseDto> ComandaResponseDtoList(List<DOM.Comanda> comandas)
        {
            return comandas.Select(c => ComandaResponseDto(c)).ToList();
        }



    }
}
