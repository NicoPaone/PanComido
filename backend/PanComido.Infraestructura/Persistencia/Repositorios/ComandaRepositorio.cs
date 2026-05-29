using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Infraestructura.Persistencia.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Infraestructura.Persistencia.Repositorios
{
    public class ComandaRepositorio : IComandaRepositorio
    {
        private readonly AppDbContext _ctx;
        private readonly ComandaEntityMapper _mapper;

        public ComandaRepositorio(AppDbContext context, ComandaEntityMapper mapper)
        {
            _ctx = context;
            _mapper = mapper;
        }
        public async Task<List<Comanda>> ObtenerComandasActivasAsync(int restauranteId)

        {
            var efList = await  _ctx.Comanda
                .Include   (c => c.EstadoComanda)
                .Include   (c => c.ArticuloComanda)
                    .ThenInclude(ac => ac.Articulo)
                    .ThenInclude (a => a.Plato)
                    


                .Where(c => c.RestauranteId == restauranteId)

                .Where(c => c.EstadoComandaId != (int)EstadoComanda.Finalizada)

               
            
                .ToListAsync();


            return  efList.Select(C=> _mapper.ParaDominio(C)).ToList();
        }

    }

} 


    

