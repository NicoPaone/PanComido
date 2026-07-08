namespace PanComido.Dominio.Interfaces.Servicios
{
    public interface ITransaccionPersistenciaServicio
    {
        Task EjecutarAsync(Func<Task> operacion);
    }
}
