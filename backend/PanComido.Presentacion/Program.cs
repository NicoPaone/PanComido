using MercadoPago.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PanComido.Dominio.CasosDeUso.ArticuloCasosDeUso;
using PanComido.Dominio.CasosDeUso.AutenticacionCasosDeUso;
using PanComido.Dominio.CasosDeUso.AvisosCasosDeUso;
using PanComido.Dominio.CasosDeUso.AvisosCasosDeUso.IA;
using PanComido.Dominio.CasosDeUso.BodegaCasosDeUso;
using PanComido.Dominio.CasosDeUso.CartaCasosDeUso;
using PanComido.Dominio.CasosDeUso.ComandaCasosDeUso;
using PanComido.Dominio.CasosDeUso.ConfiguracionCasoDeUso;
using PanComido.Dominio.CasosDeUso.CrearPlatoCasoDeUso;
using PanComido.Dominio.CasosDeUso.Dashboard;
using PanComido.Dominio.CasosDeUso.EncuestaCasosDeUso;
using PanComido.Dominio.CasosDeUso.InsumoCasosDeUso;
using PanComido.Dominio.CasosDeUso.LlamadoMozoCasoDeUso;
using PanComido.Dominio.CasosDeUso.MesaCasosDeUso;
using PanComido.Dominio.CasosDeUso.PagoCasoDeUso;
using PanComido.Dominio.CasosDeUso.EmpleadoCasosDeUso;
using PanComido.Dominio.CasosDeUso.PedidosCasosDeUso;
using PanComido.Dominio.CasosDeUso.MiseAndPlaceCasoDeUso;
using PanComido.Dominio.CasosDeUso.PlatoCasoDeUso;
using PanComido.Dominio.CasosDeUso.PlatoCasosDeUso;
using PanComido.Dominio.CasosDeUso.ProveedorCasosDeUso;
using PanComido.Dominio.CasosDeUso.ReglaTiempoExtraCasosDeUso;
using PanComido.Dominio.CasosDeUso.ReporteCasosDeUso;
using PanComido.Dominio.CasosDeUso.UnidadMedidaCasosDeUso;
using PanComido.Dominio.Interfaces.Repositorios;
using PanComido.Dominio.Interfaces.Repositorios.IA;
using PanComido.Dominio.Interfaces.Servicios;
using PanComido.Dominio.Interfaces.Servicios.IA;
using PanComido.Dominio.Interfaces.Servicios.MercadoPago;
using PanComido.Dominio.Servicios;
using PanComido.Infraestructura.Persistencia;
using PanComido.Infraestructura.Persistencia.Mappers;
using PanComido.Infraestructura.Persistencia.Mappers.IA;
using PanComido.Infraestructura.Persistencia.Repositorios;
using PanComido.Infraestructura.Persistencia.Repositorios.IA;
using PanComido.Infraestructura.ServiciosExternos;
using PanComido.Infraestructura.ServiciosExternos.Gemini;
using PanComido.Infraestructura.ServiciosExternos.Gemini.Mappers;
using PanComido.Infraestructura.ServiciosExternos.Gemini.Servicio;
using PanComido.Infraestructura.ServiciosExternos.MercadoPago;
using PanComido.Presentacion;
using PanComido.Presentacion.Filtros;
using PanComido.Presentacion.Hubs;
using PanComido.Presentacion.Mappers;
using PanComido.Presentacion.Mappers.Dashboard;
using PanComido.Presentacion.Servicios;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using QuestPDF.Infrastructure;

QuestPDF.Settings.License = LicenseType.Community;
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddControllers();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<RestauranteContextoFilter>();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{  
   options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
   {
      Name = "Authorization",
      Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
      Scheme = "Bearer",
      BearerFormat = "JWT",
      In = Microsoft.OpenApi.Models.ParameterLocation.Header,
      Description = "Pegá el token JWT acá"
   });
   
   options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
      {
         new Microsoft.OpenApi.Models.OpenApiSecurityScheme
         {
            Reference = new Microsoft.OpenApi.Models.OpenApiReference
            {
               Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
               Id = "Bearer"
            }
         },
         Array.Empty<string>()
      }
   });
}); 

builder.Services.AddSignalR();

// Conexion a BD
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


//  JWT
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
   .AddJwtBearer(options =>
   {
      options.TokenValidationParameters = new TokenValidationParameters
      {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ValidIssuer = builder.Configuration["Jwt:Issuer"],
         ValidAudience = builder.Configuration["Jwt:Audience"],
         IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
      };
   });
builder.Services.AddAuthorization();

//AUTENTICACION
builder.Services.AddScoped<JwtTokenServicio>();
builder.Services.AddScoped<AutenticacionMapper>();
builder.Services.AddScoped<LoginCasoDeUso>();
builder.Services.AddScoped<IEmpleadoRepositorio, EmpleadoRepositorio>();
builder.Services.AddScoped<IContraseniaHasher, ContraseniaHasher>();

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
builder.Services.AddScoped<MetodoDePagoEntityMapper>();
builder.Services.AddScoped<RestauranteEntityMapper>();
builder.Services.AddScoped<TurnoLaboralEntityMapper>();
builder.Services.AddScoped<FilaVirtualEntityMapper>();
builder.Services.AddScoped<FamiliaTipograficaEntityMapper>();
builder.Services.AddScoped<PorcentajesCategoriaEntityMapper>();
builder.Services.AddScoped<PagoEntityMapper>();
builder.Services.AddScoped<DatosTransferenciaEntityMapper>();
builder.Services.AddScoped<ReglaTiempoExtraEntityMapper>();



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
builder.Services.AddScoped<CartaComensalMapper>();
builder.Services.AddScoped<FormularioParaCrearPlatoMapper>();
builder.Services.AddScoped<PlatoMapper>();
builder.Services.AddScoped<ArticuloCartaMapper>();
builder.Services.AddScoped<MetodoDePagoMapper>();
builder.Services.AddScoped<RestauranteMapper>();
builder.Services.AddScoped<TurnoLaboralMapper>();
builder.Services.AddScoped<FilaVirtualMapper>();
builder.Services.AddScoped<DashboardMapper>();
builder.Services.AddScoped<PlatoAnalisisMapper>();
builder.Services.AddScoped<FamiliaTipograficaMapper>();
builder.Services.AddScoped<PorcentajesGananciaMapper>();
builder.Services.AddScoped<ArticuloMapper>();
builder.Services.AddScoped<PagoMapper>();
builder.Services.AddScoped<DatosBienvenidaMesaMapper>();
builder.Services.AddScoped<EncuestaMapper>();
builder.Services.AddScoped<DatosTransferenciaMapper>();
builder.Services.AddScoped<MiseAndPlaceMapper>();
builder.Services.AddScoped<EmpleadoMapper>();
builder.Services.AddScoped<ReglaTiempoExtraMapper>();

// Repositorios
builder.Services.AddScoped<IInsumoRepositorio, InsumoRepositorio>();
builder.Services.AddScoped<ILoteRepositorio, LoteRepositorio>();
builder.Services.AddScoped<IBodegaRepositorio, BodegaRepositorio>();
builder.Services.AddScoped<IProveedorRepositorio, ProveedorRepositorio>();
builder.Services.AddScoped<IPedidoRepositorio, PedidoRepositorio>();
builder.Services.AddScoped<IComandaRepositorio, ComandaRepositorio>();
builder.Services.AddScoped<IDashboardRepositorio, DashboardRepositorio>();
builder.Services.AddScoped<ICategoriaInsumoRepositorio, CategoriaInsumoRepositorio>();
builder.Services.AddScoped<IUnidadMedidaRepositorio, UnidadMedidaRepositorio>();
builder.Services.AddScoped<IMesaRepositorio, MesaRepositorio>();
builder.Services.AddScoped<IArticuloRepositorio, ArticuloRepositorio>();
builder.Services.AddScoped<IPlatoAnalisisRepositorio, PlatoAnalisisRepositorio>();
builder.Services.AddScoped<ILlamadoRepositorio, LlamadoRepositorio>();
builder.Services.AddScoped<IMozoRepositorio, MozoRepositorio>();
builder.Services.AddScoped<IFormularioPlatoRepositorio, FormularioPlatoRepositorio>();
builder.Services.AddScoped<IPlatoRepositorio, PlatoRepositorio>();
builder.Services.AddScoped<IPagoRepositorio, PagoRepositorio>();
builder.Services.AddScoped<IMetodoDePagoRepositorio, MetodoDePagoRepositorio>();
builder.Services.AddScoped<IRestauranteRepositorio, RestauranteRepositorio>();
builder.Services.AddScoped<ITurnoLaboralRepositorio, TurnoLaboralRepositorio>();
builder.Services.AddScoped<IFilaVirtualRepositorio, FilaVirtualRepositorio>();
builder.Services.AddScoped<IFamiliaTipograficaRepositorio, FamiliaTipograficaRepositorio>();
builder.Services.AddScoped<IPorcentajesCategoriaRepositorio, PorcentajesGananciaRepositorio>();
builder.Services.AddScoped<IDatosTransferenciaRepositorio, DatosTransferenciaRepositorio>();
builder.Services.AddScoped<IEncuestaSatisfaccionRepositorio, EncuestaSatisfaccionRepositorio>();
builder.Services.AddScoped<IMiseAndPlaceRepositorio, MiseAndPlaceRepositorio>();
builder.Services.AddScoped<IReglaTiempoExtraRepositorio, ReglaTiempoExtraRepositorio>();


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
builder.Services.AddScoped<ObtenerComandaActivaPorMesaCasoDeUso>();
builder.Services.AddScoped<ObtenerInsumosParaPedidoCasoDeUso>();
builder.Services.AddScoped<OcuparMesaCasoDeUso>();
builder.Services.AddScoped<EnviarPedidoProveedorCasoDeUso>();
builder.Services.AddScoped<GenerarSugerenciasRecepcionCasoDeUso>();
builder.Services.AddScoped<RecibirPedidoProveedorCasoDeUso>();
builder.Services.AddScoped<ListarMesasCasoDeUso>();
builder.Services.AddScoped<GuardarMapaCasoDeUso>();
builder.Services.AddScoped<ObtenerDatosParaFormularioCrearPlato>();
builder.Services.AddScoped<CrearPlatoCasoDeUso>();
builder.Services.AddScoped<ModificarPlatoCasoDeUso>();
builder.Services.AddScoped<EliminarPlatoCasoDeUso>();
builder.Services.AddScoped<ObtenerPlatoPorIdCasoDeUso>();
builder.Services.AddScoped<ObtenerCartaComensalCasoDeUso>();
builder.Services.AddScoped<ListarInsumosConStockCriticoCasoDeUso>();
builder.Services.AddScoped<ListarInsumosConVencimientoProximoCasoDeUso>();
builder.Services.AddScoped<LlamarMozoCasoDeUso>();
builder.Services.AddScoped<ListarLlamadosPendientesCasoDeUso>();
builder.Services.AddScoped<ResolverLlamadoCasoDeUso>();
builder.Services.AddScoped<ListarComandasActivasMozoCasoDeUso>();
builder.Services.AddScoped<ObtenerDetalleArticuloCasoDeUso>();
builder.Services.AddScoped<MarcarItemsEntregadosCasoDeUso>();
builder.Services.AddScoped<SolicitarPagoCasoDeUso>();
builder.Services.AddScoped<ConfirmarPagoCasoDeUso>();
builder.Services.AddScoped<ConfirmarPedidoClienteAComandaCasoDeUso>();
builder.Services.AddScoped<ObtenerArticulosParaCrearCartaCasoDeUso>();
builder.Services.AddScoped<ModificarArticuloCasoDeUso>();
builder.Services.AddScoped<LlamarMozoComandaCasoDeUso>();
builder.Services.AddScoped<CambiarEstadoMesaCasoDeUso>();
builder.Services.AddScoped<CrearProveedorCasoDeUso>();
builder.Services.AddScoped<ModificarProveedorCasoDeUso>();
builder.Services.AddScoped<EliminarProveedorCasoDeuso>();
builder.Services.AddScoped<ObtenerProveedorCasoDeUso>();
builder.Services.AddScoped<ObtenerVencimientosYCriticidadDashboardCasoDeUso>();
builder.Services.AddScoped<ObtenerRendimientoComercialCasoDeUso>();
builder.Services.AddScoped<ObtenerResumenOperativoCasoDeUso>();
builder.Services.AddScoped<ObtenerIngredientesExcluidosStatsCasoDeUso>();
builder.Services.AddScoped<ObtenerAnalisisPlatoCasoDeUso>();
builder.Services.AddScoped<AplicarDescuentoCasoDeUso>();
builder.Services.AddScoped<AgendarRecordatorioCasoDeUso>();
builder.Services.AddScoped<ResolverNotificacionCasoDeUso>();
builder.Services.AddScoped<ObtenerMetodosDePagoCasoDeUso>();
builder.Services.AddScoped<ActualizarMetodosDePagoCasoDeUso>();
builder.Services.AddScoped<ObtenerDatosDelLocalCasoDeUso>();
builder.Services.AddScoped<ActualizarDatosDelLocalCasoDeUso>();
builder.Services.AddScoped<ObtenerTurnosLaboralesCasoDeUso>();
builder.Services.AddScoped<ActualizarTurnosLaboralesCasoDeUso>();
builder.Services.AddScoped<ObtenerFilaVirtualCasoDeUso>();
builder.Services.AddScoped<ActualizarFilaVirtualCasoDeUso>();
builder.Services.AddScoped<ListarFamiliasTipograficasCasoDeUso>();
builder.Services.AddScoped<ObtenerDatosMesaBienvenidaCasoDeUso>();
builder.Services.AddScoped<ObtenerPorcentajesCasoDeUso>();
builder.Services.AddScoped<ActualizarPorcentajesCasoDeUso>();
builder.Services.AddScoped<CrearPreferenciaMPCasoDeUso>();
builder.Services.AddScoped<ConfirmarPagoMPCasoDeUso>();
builder.Services.AddScoped<GenerarSugerenciasPlatoIACasoDeUso>();
builder.Services.AddScoped<ObtenerDatosInvitadoBienvenidaAComandaCasoDeUso>();
builder.Services.AddScoped<AsignarMozosMesaCasoDeUso>();
builder.Services.AddScoped<DesasignarMozoMesaCasoDeUso>();
builder.Services.AddScoped<ListarMozosParaMesaCasoDeUso>();
builder.Services.AddScoped<ObtenerDatosTransferenciaCasoDeUso>();
builder.Services.AddScoped<ActualizarDatosTransferenciaCasoDeUso>();
builder.Services.AddScoped<ObtenerResumenSatisfaccionCasoDeUso>();
builder.Services.AddScoped<CrearEncuestaSatisfaccionCasoDeUso>();
builder.Services.AddScoped<ModificarInsumoCasoDeUso>();
builder.Services.AddScoped<ObtenerInsumoPorIdCasoDeUso>();
builder.Services.AddScoped<EliminarInsumoCasoDeUso>();
builder.Services.AddScoped<CrearEncuestaSatisfaccionCasoDeUso>();


builder.Services.AddScoped<CrearEncuestaSatisfaccionCasoDeUso>();
builder.Services.AddScoped<ObtenerIngredientesParaCrearMiseAndPlace>();
builder.Services.AddScoped<CrearMiseAndPlaceCasoDeUso>();
builder.Services.AddScoped<ObtenerTodosLosMiseAndPlaceCasoDeUso>();
builder.Services.AddScoped<ObtenerMiseAndPlacePorIdCasoDeUso>();
builder.Services.AddScoped<ListarEmpleadosCasoDeUso>();
builder.Services.AddScoped<CrearEmpleadoCasoDeUso>();
builder.Services.AddScoped<ModificarEmpleadoCasoDeUso>();
builder.Services.AddScoped<EliminarEmpleadoCasoDeUso>();

builder.Services.AddScoped<GenerarReporteDashboardPdfCasoDeUso>();
builder.Services.AddScoped<GenerarReportePersonalPdfCasoDeUso>();
builder.Services.AddScoped<GenerarReporteVentasPdfCasoDeUso>();

builder.Services.AddScoped<ObtenerReglasTiempoExtraCasoDeUso>();
builder.Services.AddScoped<CrearReglaTiempoExtraCasoDeUso>();
builder.Services.AddScoped<ModificarReglaTiempoExtraCasoDeUso>();
builder.Services.AddScoped<EliminarReglaTiempoExtraCasoDeUso>();

// Servicios
builder.Services.AddScoped<IEstadoStockInsumoServicio, EstadoStockInsumoServicio>();
builder.Services.AddScoped<IInsumoValidacionServicio, InsumoValidacionServicio>();
builder.Services.AddScoped<IDisponibilidadArticuloServicio, DisponibilidadArticuloServicio>();
builder.Services.AddScoped<ISugerenciaPlatosIAServicio, GeminiSugerenciaPlatosIAServicio >();
builder.Services.AddScoped<IGestionStockServicio, GestionStockServicio>();
builder.Services.AddScoped<IVencimientosProximosInsumosServicio, VencimientosProximosInsumosServicio>();
builder.Services.AddScoped<ITiempoDePreparacionPlatoServicio, TiempoDePreparacionPlatoServicio>();
builder.Services.AddScoped<ICalculadorCostoPlatoServicio, CalculadorCostoPlatoServicio>();
builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddScoped<ICalcularTotalComandaServicio, CalcularTotalComandaServicio>();
builder.Services.AddScoped<ICrearLlamadoServicio, CrearLlamadoServicio>();
builder.Services.AddScoped<IRegistrarPagoServicio,  RegistrarPagoServicio>();
builder.Services.AddScoped<IVerificarMetodoPagoHabilitadoServicio, VerificarMetodoPagoHabilitadoServicio>();
builder.Services.AddScoped<IPdfGeneradorServicio, QuestPdfGeneradorServicio>();
builder.Services.AddScoped<IGeneradorNombreLoteServicio, GeneradorNombreLoteServicio>();


//Servicios externos
builder.Services.AddScoped<IComandaNotificador, ComandaNotificadorSignalR>();
builder.Services.AddScoped<ILlamadoNotificador, LlamadoNotificadorSignalR>();
builder.Services.AddScoped<IMesaNotificador, MesaNotificadorSignalR>();
builder.Services.Configure<GeminiConfiguracion>(builder.Configuration.GetSection("Gemini"));
builder.Services.AddScoped<GeminiResponseMapper>();
builder.Services.AddScoped<SugerenciaIAEntityMapper>();
builder.Services.AddScoped<ISugerenciaIARepositorio, SugerenciaIARepositorio>();
builder.Services.AddScoped<IImagenServicio,CloudinaryImagenServicio>();
builder.Services.AddScoped<IMercadoPagoServicio, MercadoPagoServicio>();

//MERCADO PAGO
MercadoPagoConfig.AccessToken = builder.Configuration["MercadoPago:AccessToken"];
builder.Services.Configure<MercadoPagoConfiguracion>(builder.Configuration.GetSection("MercadoPago"));

// Excepcion Handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddHttpClient();

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

// app.UseHttpsRedirection();

app.UseExceptionHandler(o => { });

app.UseCors("ProduccionCors");


app.UseAuthentication();
app.UseAuthorization();

// para imagenes
app.UseStaticFiles();

app.MapControllers();

//mapear el hub de SignalR
app.MapHub<PanComidoHub>("/hubs/pancomido");

app.Run();
