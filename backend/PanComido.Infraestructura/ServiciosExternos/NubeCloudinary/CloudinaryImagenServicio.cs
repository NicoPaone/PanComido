using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using PanComido.Dominio.Interfaces.Servicios;

namespace PanComido.Infraestructura.ServiciosExternos;

public class CloudinaryImagenServicio : IImagenServicio
{
   private readonly Cloudinary _cloudinary;

   public CloudinaryImagenServicio(IConfiguration configuration)
   {
      var url = configuration["Cloudinary:Url"]
          ?? throw new InvalidOperationException("Cloudinary:Url no configurada");

      var uri = new Uri(url);
      var apiKey = uri.UserInfo.Split(':')[0];
      var apiSecret = uri.UserInfo.Split(':')[1];
      var cloudName = uri.Host;

      _cloudinary = new Cloudinary(new Account(cloudName, apiKey, apiSecret));
      _cloudinary.Api.Secure = true;
   }

   public async Task<string> SubirImagenAsync(Stream archivo, string nombre, string carpeta)
   {
      var uploadParams = new ImageUploadParams
      {
         File = new FileDescription(nombre, archivo),
         Folder = carpeta,
         UseFilename = true,
         UniqueFilename = true
      };

      var result = await _cloudinary.UploadAsync(uploadParams);

      if (result.Error != null)
         throw new InvalidOperationException($"Error al subir imagen: {result.Error.Message}");

      return result.SecureUrl.ToString();
   }

   public Task EliminarImagenAsync(string url) => Task.CompletedTask;
}