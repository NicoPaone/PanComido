using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.CasosDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Infraestructura.Persistencia;
using PanComido.Infraestructura.Persistencia.Mappers;
using PanComido.Infraestructura.Persistencia.Repositorios;
using PanComido.Presentacion.Mappers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Conexion a BD
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Mappers de Infraestructura (Dominio <-> EF)
builder.Services.AddScoped<InsumoEntityMapper>();

// Mappers de Presentacion (Dominio <-> DTOs)   
builder.Services.AddScoped<InsumoMapper>();

// Repositorios
builder.Services.AddScoped<IInsumoRepositorio, InsumoRepositorio>();
builder.Services.AddScoped<ILoteRepositorio, LoteRepositorio>();

// Casos de uso
builder.Services.AddScoped<ListarInsumoCasoDeUso>();


var allowedOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();



builder.Services.AddCors(options =>
{
   options.AddPolicy("ProduccionCors", policy =>
   {
      policy.WithOrigins(allowedOrigins)
      .AllowAnyMethod()
      .AllowAnyHeader();
   });

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("ProduccionCors");

app.UseAuthorization();

app.MapControllers();

app.Run();
