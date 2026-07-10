using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.CasosDeUso.EncuestaCasosDeUso
{
    public class CrearEncuestaSatisfaccionCasoDeUso
    {
        private readonly IEncuestaSatisfaccionRepositorio _encuestaRepo;
        private readonly IComandaRepositorio _comandaRepo;
        private readonly IRestauranteRepositorio _restauranteRepo;
        public CrearEncuestaSatisfaccionCasoDeUso(
            IEncuestaSatisfaccionRepositorio encuestaRepo,
            IComandaRepositorio comandaRepo,
            IRestauranteRepositorio restauranteRepo)
        {
            _encuestaRepo = encuestaRepo;
            _comandaRepo = comandaRepo;
            _restauranteRepo = restauranteRepo;
        }
        public async Task<string?> EjecutarAsync(EncuestaSatisfaccion encuesta)
        {
            Comanda comanda = await ObtenerYValidarComanda(encuesta.ComandaId);

            encuesta.Fecha = DateTime.UtcNow;

            await _encuestaRepo.GuardarEncuestaAsync(encuesta);
            
            double promedio = (encuesta.PuntuacionLugar + encuesta.PuntuacionComida + encuesta.PuntuacionMozo) / 3.0;

            return await ObtenerLinkResenaGoogleMapsEnBaseAPromedio(promedio, comanda.RestauranteId); 
        }

        private async Task<Comanda> ObtenerYValidarComanda(int comandaId)
        {
            var comanda = await _comandaRepo.ObtenerComandaPorIdAsync(comandaId);
            if (comanda == null) throw new KeyNotFoundException("Comanda no encontrada.");
            if (comanda.Estado != EstadoComanda.Finalizada) throw new ArgumentException("La comanda aún no se puede calificar porque no finalizó.");
            return comanda; 
        }

        private async Task<string?> ObtenerLinkResenaGoogleMapsEnBaseAPromedio(double promedio, int restauranteId)
        {
            if (promedio >= 4.0)
            {
                var restaurante = await _restauranteRepo.ObtenerDatosDelLocalAsync(restauranteId);
                return restaurante.LinkResenaGoogleMaps;
            }
            return null;
        }
    }
}
