using PanComido.Dominio.Entidades;
using PanComido.Presentacion.DTOs;
using DOM = PanComido.Dominio.Entidades;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

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
                cantComensales = comanda.CantComensales,
                Estado = comanda.Estado.ToString(),
                HoraInicio = comanda.HoraInicio.ToString("dd/MM/yyyy HH:mm"),
                HoraFin = comanda.HoraFin?.ToString("dd/MM/yyyy HH:mm"),

                TiempoEstimadoTotal = comanda.Items
                    .Select(i => i.Articulo)
                    .OfType<Plato>()
                    .Select(plato => plato.TiempoPreparacionBase)
                    .DefaultIfEmpty(0)
                    .Max(),

                Platos = comanda.Items.Select(p => new PlatoDto
                {

                    Nombre = p.Articulo.Nombre,
                    Cantidad = p.Cantidad,
                    ObservacionesGenerales = p.ObservacionesGenerales,
                    ObservacionesIngredientes = p.ObservacionesIngredientes,

                }).ToList()


            };

        }


        public List<ComandaResponseDto> ComandaResponseDtoList(List<DOM.Comanda> comandas)
        {
            return comandas.Select(c => ComandaResponseDto(c)).ToList();


        }



    }
}
