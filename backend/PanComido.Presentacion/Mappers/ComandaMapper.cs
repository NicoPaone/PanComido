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

                TiempoEstimadoTotal = comanda.Platos.Any()
                ? comanda.Platos.Max ( p => p.TiempoPreparacionBase ) : 0,

                Platos = comanda.Platos.Select(p => new PlatoDto
                {
                   
                    Nombre = p.Nombre,
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
