using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;

namespace PanComido.Dominio.Servicios
{
    public class CrearLlamadoServicio : ICrearLlamadoServicio
    {
        private readonly ILlamadoRepositorio _llamadoRepositorio;
        private readonly ILlamadoNotificador _llamadoNotificador;

        public CrearLlamadoServicio(ILlamadoRepositorio llamadoRepositorio, ILlamadoNotificador llamadoNotificador)
        {
            _llamadoRepositorio = llamadoRepositorio;
            _llamadoNotificador = llamadoNotificador;
        }

        public async Task<Llamado> CrearYNotificarAsync(int? mozoId, int mesaId, int numeroDeMesa, CategoriaLlamado categoriaLlamadoId, string? descripcion)
        {
            var llamado = new Llamado
            {
                MozoId = mozoId,
                MesaId = mesaId,
                NumeroDeMesa = numeroDeMesa,
                CategoriaLlamadoId = (int)categoriaLlamadoId,
                Descripcion = descripcion,
                Resuelto = false
            };

            var llamadoGuardado = await _llamadoRepositorio.crearLlamadoAsync(llamado);

            await _llamadoNotificador.NotificarLlamadoAsync(llamadoGuardado);

            return llamadoGuardado;
        }
    }
}
