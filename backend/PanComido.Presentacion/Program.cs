using Microsoft.EntityFrameworkCore;
using PanComido.Dominio.CasosDeUso.AvisosCasosDeUso;
using PanComido.Dominio.CasosDeUso.BodegaCasosDeUso;
using PanComido.Dominio.CasosDeUso.CartaCasosDeUso;
using PanComido.Dominio.CasosDeUso.ComandaCasosDeUso;
using PanComido.Dominio.CasosDeUso.InsumoCasosDeUso;
using PanComido.Dominio.CasosDeUso.LlamadoMozoCasoDeUso;
using PanComido.Dominio.CasosDeUso.MesaCasosDeUso;
using PanComido.Dominio.CasosDeUso.PedidosCasosDeUso;
using PanComido.Dominio.CasosDeUso.ProveedorCasosDeUso;
using PanComido.Dominio.CasosDeUso.UnidadMedidaCasosDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Servicios;
using PanComido.Dominio.Servicios;
using PanComido.Infraestructura.Persistencia;
using PanComido.Infraestructura.Persistencia.Mappers;
using PanComido.Infraestructura.Persistencia.Repositorios;
using PanComido.Presentacion;
using PanComido.Presentacion.Hubs;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.Servicios;
using PanComido.Presentacion.SesionMock;
using PanComido.Dominio.CasosDeUso.CrearPlatoCasoDeUso;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddControllers();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<RestauranteContextoFilter>();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

// Conexion a BD
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Mappers de Infraestructura (Dominio <-> EF)
builder.Services.AddScoped<InsumoEntityMapper>();
builder.Services.AddScoped<BodegaEntityMapper>();
builder.Services.AddScoped<ProveedorEntityMapper>();
builder.Services.AddScoped<PedidoEntityMapper>();
builder.Services.AddScoped<ComandaEntityMapper>();
builder.Services.AddScoped<CategoriaInsumoEntityMapper>();
builder.Services.AddScoped<UnidadMedidaEntityMapper>();
builder.Services.AddScoped<MesaEntityMapper>();
builder.Services.AddScoped<LoteEntityMapper>();
builder.Services.AddScoped<IngredientePreparadoEntityMapper>();
builder.Services.AddScoped<FormularioParaCrearPlatoEntityMapper>();
builder.Services.AddScoped<ArticuloEntityMapper>();
builder.Services.AddScoped<PlatoEntityMapper>();
builder.Services.AddScoped<LlamadoEntityMapper>();



// Mappers de Presentacion (Dominio <-> DTOs)   
builder.Services.AddScoped<InsumoMapper>();
builder.Services.AddScoped<BodegaMapper>();
builder.Services.AddScoped<ProveedorMapper>();
builder.Services.AddScoped<PedidoMapper>();
builder.Services.AddScoped<ComandaMapper>();
builder.Services.AddScoped<CategoriaInsumoMapper>();
builder.Services.AddScoped<InsumoConsugerenciaMapper>();
builder.Services.AddScoped<UnidadMedidaMapper>();
builder.Services.AddScoped<LoteRecepcionMapper>();
builder.Services.AddScoped<MesaMapper>();

builder.Services.AddScoped<LoteMapper>();
builder.Services.AddScoped<LlamadoMapper>();
builder.Services.AddScoped<CartaMapper>();
builder.Services.AddScoped<FormularioParaCrearPlatoMapper>();


// Repositorios
builder.Services.AddScoped<IInsumoRepositorio, InsumoRepositorio>();
builder.Services.AddScoped<ILoteRepositorio, LoteRepositorio>();
builder.Services.AddScoped<IBodegaRepositorio, BodegaRepositorio>();
builder.Services.AddScoped<IProveedorRepositorio, ProveedorRepositorio>();
builder.Services.AddScoped<IPedidoRepositorio, PedidoRepositorio>();
builder.Services.AddScoped<IComandaRepositorio, ComandaRepositorio>();
builder.Services.AddScoped<ICategoriaInsumoRepositorio, CategoriaInsumoRepositorio>();
builder.Services.AddScoped<IUnidadMedidaRepositorio, UnidadMedidaRepositorio>();
builder.Services.AddScoped<IMesaRepositorio, MesaRepositorio>();
builder.Services.AddScoped<IArticuloRepositorio, ArticuloRepositorio>();
builder.Services.AddScoped<ILlamadoRepositorio, LlamadoRepositorio>();
builder.Services.AddScoped<IMozoRepositorio, MozoRepositorio>();
builder.Services.AddScoped<IFormularioPlatoRepositorio, FormularioPlatoRepositorio>();


// Casos de uso
builder.Services.AddScoped<ListarInsumoCasoDeUso>();
builder.Services.AddScoped<CrearInsumoCasoDeUso>();
builder.Services.AddScoped<ListarProveedorCasoDeUso>();
builder.Services.AddScoped<ObtenerHistorialPedidosCasoDeUso>();
builder.Services.AddScoped<ListarBodegasCasoDeUso>();
builder.Services.AddScoped<ListarBodegasConInsumosCasoDeUso>();
builder.Services.AddScoped<ListarInsumosDelProveedorCasoDeUso>();
builder.Services.AddScoped<CrearPedidoCasoDeUso>();
builder.Services.AddScoped<ListarComandaActivaCocinaCasoDeUso>();
builder.Services.AddScoped<ListarCategoriasDeInsumosCasoDeUso>();
builder.Services.AddScoped<ListarUnidadesDeMedidaCasoDeUso>();
builder.Services.AddScoped<ModificarEstadoComandaCasoDeUso>();
builder.Services.AddScoped<ObtenerInsumosParaPedidoCasoDeUso>();
builder.Services.AddScoped<OcuparMesaCasoDeUso>();
builder.Services.AddScoped<EnviarPedidoProveedorCasoDeUso>();
builder.Services.AddScoped<GenerarSugerenciasRecepcionCasoDeUso>();
builder.Services.AddScoped<RecibirPedidoProveedorCasoDeUso>();
builder.Services.AddScoped<ListarMesasCasoDeUso>();
builder.Services.AddScoped<ObtenerDatosParaFormularioCrearPlato>();
builder.Services.AddScoped<ObtenerCartaCasoDeUso>();
builder.Services.AddScoped<ListarInsumosConStockCriticoCasoDeUso>();
builder.Services.AddScoped<ListarInsumosConVencimientoProximoCasoDeUso>();
builder.Services.AddScoped<LlamarMozoCasoDeUso>();
builder.Services.AddScoped<ListarLlamadosPendientesCasoDeUso>();
builder.Services.AddScoped<ResolverLlamadoCasoDeUso>();
builder.Services.AddScoped<ListarComandasActivasMozoCasoDeUso>();
builder.Services.AddScoped<MarcarItemEntregadoCasoDeUso>();

// Servicios
builder.Services.AddScoped<IEstadoStockInsumoServicio, EstadoStockInsumoServicio>();
builder.Services.AddScoped<IDisponibilidadArticuloServicio, DisponibilidadArticuloServicio>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

//Servicios externos
builder.Services.AddScoped<IComandaNotificador, ComandaNotificadorSignalR>();
builder.Services.AddScoped<ILlamadoNotificador, LlamadoNotificadorSignalR>();

var allowedOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();



builder.Services.AddCors(options =>
{
    options.AddPolicy("ProduccionCors", policy =>
    {
        policy.WithOrigins(allowedOrigins)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
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

app.UseExceptionHandler(o => { });

app.UseCors("ProduccionCors");

app.UseAuthorization();

app.MapControllers();

//mapear el hub de SignalR
app.MapHub<PanComidoHub>("/hubs/pancomido");

app.Run();