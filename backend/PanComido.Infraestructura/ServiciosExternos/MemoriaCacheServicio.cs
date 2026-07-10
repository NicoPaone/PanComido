using Microsoft.Extensions.Caching.Memory;
using PanComido.Dominio.Interfaces.Servicios;
using System;

namespace PanComido.Infraestructura.ServiciosExternos
{
    public class MemoriaCacheServicio : ICacheServicio
    {
        private readonly IMemoryCache _memoryCache;

        public MemoriaCacheServicio(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            return _memoryCache.TryGetValue(key, out value);
        }

        public void Set<T>(string key, T value, TimeSpan absoluteExpirationRelativeToNow)
        {
            _memoryCache.Set(key, value, absoluteExpirationRelativeToNow);
        }
    }
}
