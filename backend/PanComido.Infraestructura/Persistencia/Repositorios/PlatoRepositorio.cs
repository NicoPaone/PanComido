using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Infraestructura.Persistencia.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    public class PlatoRepositorio : IPlatoRepositorio

    {
        private readonly AppDbContext _ctx;
        private readonly ArticuloEntityMapper _articuloMapper;


        public PlatoRepositorio(AppDbContext ctx, ArticuloEntityMapper articuloMapper)
        {
            _ctx = ctx;
            _articuloMapper = articuloMapper;
        }


        public async Task CrearAsync(Dominio.Entidades.Plato platoDominio)
        {
            var efArticulo = _articuloMapper.paraEntidad(platoDominio);

            // 2. Relación Muchos-a-Muchos: Restricciones
            // Entity Framework necesita que busquemos las restricciones existentes en la BD
            // para "atarlas" al plato nuevo, en lugar de intentar crear restricciones nuevas.
            if (platoDominio.Restricciones != null && platoDominio.Restricciones.Any())
            {
                var idsRestricciones = platoDominio.Restricciones.Select(r => r.Id).ToList();
                var restriccionesDb = await _ctx.Restriccions
                                                .Where(r => idsRestricciones.Contains(r.Id))
                                                .ToListAsync();

                efArticulo.Plato.Restriccions = restriccionesDb;
            }


            await _ctx.Articulos.AddAsync(efArticulo);
            await _ctx.SaveChangesAsync();


        }
    }

    }
