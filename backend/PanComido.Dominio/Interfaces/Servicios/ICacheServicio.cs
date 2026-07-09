using System;

namespace PanComido.Dominio.Interfaces.Servicios
{
    public interface ICacheServicio
    {
        bool TryGetValue<T>(string key, out T value);
        void Set<T>(string key, T value, TimeSpan absoluteExpirationRelativeToNow);
    }
}
