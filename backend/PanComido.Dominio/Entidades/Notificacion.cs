using System;

namespace PanComido.Dominio.Entidades
{
    public class Notificacion
    {
        public int Id { get; set; }
        public int RestauranteId { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public bool Resuelta { get; set; }

        public (string Titulo, string Detalle) ObtenerEstructura()
        {
            if (string.IsNullOrWhiteSpace(Descripcion))
                return (string.Empty, string.Empty);

            if (Descripcion.StartsWith("{") && Descripcion.EndsWith("}"))
            {
                try
                {
                    using (var jsonDoc = System.Text.Json.JsonDocument.Parse(Descripcion))
                    {
                        string t = jsonDoc.RootElement.GetProperty("titulo").GetString() ?? string.Empty;
                        string d = jsonDoc.RootElement.GetProperty("detalle").GetString() ?? string.Empty;
                        return (t, d);
                    }
                }
                catch
                {
                    // Fallback si no es JSON válido
                }
            }

            int separatorIndex = Descripcion.IndexOf(" - ");
            if (separatorIndex >= 0)
            {
                return (Descripcion.Substring(0, separatorIndex), Descripcion.Substring(separatorIndex + 3));
            }

            return (Descripcion, string.Empty);
        }
    }
}

