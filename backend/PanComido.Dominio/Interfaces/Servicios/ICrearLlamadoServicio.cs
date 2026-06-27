using PanComido.Dominio.Entidades;
using PanComido.Dominio.Entidades.Enums;

namespace PanComido.Dominio.Interfaces.Servicios
{
    public interface ICrearLlamadoServicio
    {
        Task<Llamado> CrearYNotificarAsync(int? mozoId, int mesaId, int numeroDeMesa, CategoriaLlamado categoria, string? descripcion);
    }
}
