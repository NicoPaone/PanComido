using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanComido.Dominio.Interfaces.Servicios
{
   public interface IImagenServicio
   {
      Task<string> SubirImagenAsync(Stream archivo, string nombre, string carpeta);

      Task EliminarImagenAsync(string url);

   }
}
