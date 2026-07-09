using PanComido.Dominio.Interfaces.Servicios;

namespace PanComido.Dominio.Servicios
{
    public class NormalizadorNombreServicio : INormalizadorNombreServicio
    {
        public string Normalizar(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return nombre;
            string limpio = nombre.Trim().ToLower();
            return char.ToUpper(limpio[0]) + limpio.Substring(1);
        }
    }
}
