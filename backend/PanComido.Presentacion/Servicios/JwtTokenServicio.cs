using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace PanComido.Presentacion.Servicios
{
   public class JwtTokenServicio
   {
      private readonly IConfiguration _configuration;

      public JwtTokenServicio(IConfiguration configuration)
      {
         _configuration = configuration;
      }

      public string GenerarToken(int empleadoId, string email, string nombre, string rol, int restauranteId)
      {
         var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

         var claims = new[]
         {
            new Claim("sub" , empleadoId.ToString()),
            new Claim("name", nombre),
            new Claim("email", email),
            new Claim("role", rol),
            new Claim("restauranteId", restauranteId.ToString())

         };
         var credenciales = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);

         var horasDeExpiracion = int.Parse(_configuration["Jwt:ExpirationHours"] ?? "8");

         var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(horasDeExpiracion),
            signingCredentials: credenciales
            );


         return new JwtSecurityTokenHandler().WriteToken(token);


      }

   }
}
