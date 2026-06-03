using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Entidades.IA;
using PanComido.Dominio.Interfaces.Repositorios.IA;
using PanComido.Infraestructura.Persistencia.Mappers.IA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EF = PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia.Repositorios.IA
{
    public class SugerenciaIARepositorio : ISugerenciaIARepositorio
    {
        private readonly AppDbContext _ctx;
        private readonly SugerenciaIAEntityMapper _mapper;

        public SugerenciaIARepositorio(AppDbContext ctx, SugerenciaIAEntityMapper mapper)
        {
            _ctx = ctx;
            _mapper = mapper;
        }

        public async Task<SugerenciaIA?> ObtenerSugerenciaIAAsync(int restauranteId)
        {
            EF.SugerenciaPlatoIum? sugerencia = await _ctx.SugerenciaPlatoIa
                .FirstOrDefaultAsync(s => s.RestauranteId == restauranteId);

            if (sugerencia == null)
            {
                return null;
            }

            return _mapper.aDominio(sugerencia);
        }

        public async Task GuardarSugerenciaIAAsync(int restauranteId, SugerenciaIA sugerenciaIA)
        {
            EF.SugerenciaPlatoIum? sugerenciaExistente = await _ctx.SugerenciaPlatoIa
                .FirstOrDefaultAsync(s => s.RestauranteId == restauranteId);

            if (sugerenciaExistente == null)
            {
                EF.SugerenciaPlatoIum nuevaSugerencia = _mapper.aEntidad(restauranteId, sugerenciaIA);

                await _ctx.SugerenciaPlatoIa.AddAsync(nuevaSugerencia);
            }
            else
            {
                sugerenciaExistente.Json = _mapper.aJson(sugerenciaIA);
            }
            await _ctx.SaveChangesAsync();
        }
    }
}
