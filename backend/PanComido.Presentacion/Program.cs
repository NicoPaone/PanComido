using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.CasosDeUso.BodegaCasosDeUso;
using PanComido.Dominio.CasosDeUso.InsumoCasosDeUso;
using PanComido.Dominio.CasosDeUso.ProveedorCasosDeUso;
using PanComido.Dominio.Interfaces;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Servicios;
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
builder.Services.AddScoped<BodegaEntityMapper>();
builder.Services.AddScoped<ProveedorEntityMapper>();
builder.Services.AddScoped<PedidoEntityMapper>();

// Mappers de Presentacion (Dominio <-> DTOs)   
builder.Services.AddScoped<InsumoMapper>();
builder.Services.AddScoped<BodegaMapper>();
builder.Services.AddScoped<ProveedorMapper>();
builder.Services.AddScoped<PedidoMapper>();

// Repositorios
builder.Services.AddScoped<IInsumoRepositorio, InsumoRepositorio>();
builder.Services.AddScoped<ILoteRepositorio, LoteRepositorio>();
builder.Services.AddScoped<IBodegaRepositorio, BodegaRepositorio>();
builder.Services.AddScoped<IProveedorRepositorio, ProveedorRespositorio>();
builder.Services.AddScoped<IPedidoRepositorio, PedidoRepositorio>();

// Servicios
builder.Services.AddScoped<IEstadoStockInsumoServicio, EstadoStockInsumoServicio>();

// Casos de uso
builder.Services.AddScoped<ListarInsumoCasoDeUso>();
builder.Services.AddScoped<ListarBodegasCasoDeUso>();
builder.Services.AddScoped<ListarBodegasConInsumosCasoDeUso>();
builder.Services.AddScoped<ListarProveedorCasoDeUso>();
builder.Services.AddScoped<ObtenerHistorialPedidosCasoDeUso>();


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
