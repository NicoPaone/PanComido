using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PanComido.Presentacion.Persistencia.Entidades;

namespace PanComido.Presentacion.Persistencia;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Articulo> Articulos { get; set; }

    public virtual DbSet<ArticuloComandum> ArticuloComanda { get; set; }

    public virtual DbSet<Bodega> Bodegas { get; set; }

    public virtual DbSet<Cartum> Carta { get; set; }

    public virtual DbSet<CategoriaInsumo> CategoriaInsumos { get; set; }

    public virtual DbSet<CategoriaLlamado> CategoriaLlamados { get; set; }

    public virtual DbSet<CategoriaPlato> CategoriaPlatos { get; set; }

    public virtual DbSet<Cierre> Cierres { get; set; }

    public virtual DbSet<Cocina> Cocinas { get; set; }

    public virtual DbSet<Comandum> Comanda { get; set; }

    public virtual DbSet<ConfiguracionArticulo> ConfiguracionArticulos { get; set; }

    public virtual DbSet<DimensionMesa> DimensionMesas { get; set; }

    public virtual DbSet<Empleado> Empleados { get; set; }

    public virtual DbSet<EstadoComandum> EstadoComanda { get; set; }

    public virtual DbSet<EstadoMesa> EstadoMesas { get; set; }

    public virtual DbSet<EstadoPedido> EstadoPedidos { get; set; }

    public virtual DbSet<FilaVirtual> FilaVirtuals { get; set; }

    public virtual DbSet<Gerente> Gerentes { get; set; }

    public virtual DbSet<Grilla> Grillas { get; set; }

    public virtual DbSet<Ingrediente> Ingredientes { get; set; }

    public virtual DbSet<IngredienteIngredientePreparado> IngredienteIngredientePreparados { get; set; }

    public virtual DbSet<IngredientePreparado> IngredientePreparados { get; set; }

    public virtual DbSet<Insumo> Insumos { get; set; }

    public virtual DbSet<Llamado> Llamados { get; set; }

    public virtual DbSet<Lote> Lotes { get; set; }

    public virtual DbSet<Mesa> Mesas { get; set; }

    public virtual DbSet<MetodoDePago> MetodoDePagos { get; set; }

    public virtual DbSet<MetodoDePagoRestaurante> MetodoDePagoRestaurantes { get; set; }

    public virtual DbSet<Mozo> Mozos { get; set; }

    public virtual DbSet<Notificacion> Notificacions { get; set; }

    public virtual DbSet<Pago> Pagos { get; set; }

    public virtual DbSet<Pedido> Pedidos { get; set; }

    public virtual DbSet<PedidoInsumo> PedidoInsumos { get; set; }

    public virtual DbSet<Plato> Platos { get; set; }

    public virtual DbSet<PlatoIngrediente> PlatoIngredientes { get; set; }

    public virtual DbSet<Proveedor> Proveedors { get; set; }

    public virtual DbSet<Reserva> Reservas { get; set; }

    public virtual DbSet<Restaurante> Restaurantes { get; set; }

    public virtual DbSet<Restriccion> Restriccions { get; set; }

    public virtual DbSet<SugerenciaPlatoIum> SugerenciaPlatoIa { get; set; }

    public virtual DbSet<TipoBodega> TipoBodegas { get; set; }

    public virtual DbSet<TipoPlato> TipoPlatos { get; set; }

    public virtual DbSet<TurnoFila> TurnoFilas { get; set; }

    public virtual DbSet<TurnoLaboral> TurnoLaborals { get; set; }

    public virtual DbSet<Ubicacion> Ubicacions { get; set; }

    public virtual DbSet<UnidadMedidum> UnidadMedida { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Articulo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("articulo_pkey");

            entity.ToTable("articulo");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CartaId).HasColumnName("carta_id");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Eliminado)
                .HasDefaultValue(false)
                .HasColumnName("eliminado");
            entity.Property(e => e.Nombre).HasColumnName("nombre");
            entity.Property(e => e.PrecioGanancia).HasColumnName("precio_ganancia");
            entity.Property(e => e.PrecioPromocional).HasColumnName("precio_promocional");
            entity.Property(e => e.PrecioVentaFinal).HasColumnName("precio_venta_final");
            entity.Property(e => e.RestauranteId).HasColumnName("restaurante_id");
            entity.Property(e => e.UrlImagen).HasColumnName("url_imagen");

            entity.HasOne(d => d.Carta).WithMany(p => p.Articulos)
                .HasForeignKey(d => d.CartaId)
                .HasConstraintName("articulo_carta_id_fkey");

            entity.HasOne(d => d.Restaurante).WithMany(p => p.Articulos)
                .HasForeignKey(d => d.RestauranteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("articulo_restaurante_id_fkey");

            entity.HasMany(d => d.ConfiguracionArticulos).WithMany(p => p.Articulos)
                .UsingEntity<Dictionary<string, object>>(
                    "ArticuloConfiguracionArticulo",
                    r => r.HasOne<ConfiguracionArticulo>().WithMany()
                        .HasForeignKey("ConfiguracionArticuloId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("articulo_configuracion_articulo_configuracion_articulo_id_fkey"),
                    l => l.HasOne<Articulo>().WithMany()
                        .HasForeignKey("ArticuloId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("articulo_configuracion_articulo_articulo_id_fkey"),
                    j =>
                    {
                        j.HasKey("ArticuloId", "ConfiguracionArticuloId").HasName("articulo_configuracion_articulo_pkey");
                        j.ToTable("articulo_configuracion_articulo");
                        j.IndexerProperty<int>("ArticuloId").HasColumnName("articulo_id");
                        j.IndexerProperty<int>("ConfiguracionArticuloId").HasColumnName("configuracion_articulo_id");
                    });
        });

        modelBuilder.Entity<ArticuloComandum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("articulo_comanda_pkey");

            entity.ToTable("articulo_comanda");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ArticuloId).HasColumnName("articulo_id");
            entity.Property(e => e.Cantidad)
                .HasDefaultValue(1)
                .HasColumnName("cantidad");
            entity.Property(e => e.ComandaId).HasColumnName("comanda_id");
            entity.Property(e => e.Entregado)
                .HasDefaultValue(false)
                .HasColumnName("entregado");
            entity.Property(e => e.ObservacionesGenerales).HasColumnName("observaciones_generales");
            entity.Property(e => e.ObservacionesIngrediente).HasColumnName("observaciones_ingrediente");

            entity.HasOne(d => d.Articulo).WithMany(p => p.ArticuloComanda)
                .HasForeignKey(d => d.ArticuloId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("articulo_comanda_articulo_id_fkey");

            entity.HasOne(d => d.Comanda).WithMany(p => p.ArticuloComanda)
                .HasForeignKey(d => d.ComandaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("articulo_comanda_comanda_id_fkey");
        });

        modelBuilder.Entity<Bodega>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("bodega_pkey");

            entity.ToTable("bodega");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Eliminado)
                .HasDefaultValue(false)
                .HasColumnName("eliminado");
            entity.Property(e => e.Nombre).HasColumnName("nombre");
            entity.Property(e => e.RestauranteId).HasColumnName("restaurante_id");
            entity.Property(e => e.TipoBodegaId).HasColumnName("tipo_bodega_id");

            entity.HasOne(d => d.Restaurante).WithMany(p => p.Bodegas)
                .HasForeignKey(d => d.RestauranteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bodega_restaurante_id_fkey");

            entity.HasOne(d => d.TipoBodega).WithMany(p => p.Bodegas)
                .HasForeignKey(d => d.TipoBodegaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bodega_tipo_bodega_id_fkey");
        });

        modelBuilder.Entity<Cartum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("carta_pkey");

            entity.ToTable("carta");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RestauranteId).HasColumnName("restaurante_id");

            entity.HasOne(d => d.Restaurante).WithMany(p => p.Carta)
                .HasForeignKey(d => d.RestauranteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("carta_restaurante_id_fkey");
        });

        modelBuilder.Entity<CategoriaInsumo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("categoria_insumo_pkey");

            entity.ToTable("categoria_insumo");

            entity.HasIndex(e => e.Descripcion, "categoria_insumo_descripcion_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.TipoAplica).HasColumnName("tipo_aplica");

            entity.HasMany(d => d.Proveedors).WithMany(p => p.CategoriaInsumos)
                .UsingEntity<Dictionary<string, object>>(
                    "CategoriaInsumoProveedor",
                    r => r.HasOne<Proveedor>().WithMany()
                        .HasForeignKey("ProveedorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("categoria_insumo_proveedor_proveedor_id_fkey"),
                    l => l.HasOne<CategoriaInsumo>().WithMany()
                        .HasForeignKey("CategoriaInsumoId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("categoria_insumo_proveedor_categoria_insumo_id_fkey"),
                    j =>
                    {
                        j.HasKey("CategoriaInsumoId", "ProveedorId").HasName("categoria_insumo_proveedor_pkey");
                        j.ToTable("categoria_insumo_proveedor");
                        j.IndexerProperty<int>("CategoriaInsumoId").HasColumnName("categoria_insumo_id");
                        j.IndexerProperty<int>("ProveedorId").HasColumnName("proveedor_id");
                    });
        });

        modelBuilder.Entity<CategoriaLlamado>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("categoria_llamado_pkey");

            entity.ToTable("categoria_llamado");

            entity.HasIndex(e => e.Descripcion, "categoria_llamado_descripcion_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
        });

        modelBuilder.Entity<CategoriaPlato>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("categoria_plato_pkey");

            entity.ToTable("categoria_plato");

            entity.HasIndex(e => e.Descripcion, "categoria_plato_descripcion_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
        });

        modelBuilder.Entity<Cierre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cierre_pkey");

            entity.ToTable("cierre");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Diferencia).HasColumnName("diferencia");
            entity.Property(e => e.RestauranteId).HasColumnName("restaurante_id");
            entity.Property(e => e.Sobrante).HasColumnName("sobrante");
            entity.Property(e => e.TurnoLaboralId).HasColumnName("turno_laboral_id");

            entity.HasOne(d => d.Restaurante).WithMany(p => p.Cierres)
                .HasForeignKey(d => d.RestauranteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cierre_restaurante_id_fkey");

            entity.HasOne(d => d.TurnoLaboral).WithMany(p => p.Cierres)
                .HasForeignKey(d => d.TurnoLaboralId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cierre_turno_laboral_id_fkey");
        });

        modelBuilder.Entity<Cocina>(entity =>
        {
            entity.HasKey(e => e.IdEmpleado).HasName("cocina_pkey");

            entity.ToTable("cocina");

            entity.Property(e => e.IdEmpleado)
                .ValueGeneratedNever()
                .HasColumnName("id_empleado");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithOne(p => p.Cocina)
                .HasForeignKey<Cocina>(d => d.IdEmpleado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cocina_id_empleado_fkey");
        });

        modelBuilder.Entity<Comandum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("comanda_pkey");

            entity.ToTable("comanda");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CantComensales).HasColumnName("cant_comensales");
            entity.Property(e => e.EstadoComandaId).HasColumnName("estado_comanda_id");
            entity.Property(e => e.HoraFin)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("hora_fin");
            entity.Property(e => e.HoraInicio)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("hora_inicio");
            entity.Property(e => e.HoraUltimoCambioEstado)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("hora_ultimo_cambio_estado");
            entity.Property(e => e.MesaId).HasColumnName("mesa_id");
            entity.Property(e => e.PagoId).HasColumnName("pago_id");
            entity.Property(e => e.RestauranteId).HasColumnName("restaurante_id");

            entity.HasOne(d => d.EstadoComanda).WithMany(p => p.Comanda)
                .HasForeignKey(d => d.EstadoComandaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("comanda_estado_comanda_id_fkey");

            entity.HasOne(d => d.Mesa).WithMany(p => p.Comanda)
                .HasForeignKey(d => d.MesaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("comanda_mesa_id_fkey");

            entity.HasOne(d => d.Pago).WithMany(p => p.Comanda)
                .HasForeignKey(d => d.PagoId)
                .HasConstraintName("comanda_pago_id_fkey");

            entity.HasOne(d => d.Restaurante).WithMany(p => p.Comanda)
                .HasForeignKey(d => d.RestauranteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("comanda_restaurante_id_fkey");
        });

        modelBuilder.Entity<ConfiguracionArticulo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("configuracion_articulo_pkey");

            entity.ToTable("configuracion_articulo");

            entity.HasIndex(e => e.Descripcion, "configuracion_articulo_descripcion_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
        });

        modelBuilder.Entity<DimensionMesa>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("dimension_mesa_pkey");

            entity.ToTable("dimension_mesa");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Forma).HasColumnName("forma");
            entity.Property(e => e.Imagen).HasColumnName("imagen");
        });

        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("empleado_pkey");

            entity.ToTable("empleado");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Contrasena).HasColumnName("contrasena");
            entity.Property(e => e.Eliminado)
                .HasDefaultValue(false)
                .HasColumnName("eliminado");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Estado)
                .HasDefaultValueSql("'activo'::text")
                .HasColumnName("estado");
            entity.Property(e => e.Nombre).HasColumnName("nombre");
            entity.Property(e => e.RestauranteId).HasColumnName("restaurante_id");

            entity.HasOne(d => d.Restaurante).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.RestauranteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("empleado_restaurante_id_fkey");

            entity.HasMany(d => d.TurnoLaborals).WithMany(p => p.Empleados)
                .UsingEntity<Dictionary<string, object>>(
                    "EmpleadoTurnoLaboral",
                    r => r.HasOne<TurnoLaboral>().WithMany()
                        .HasForeignKey("TurnoLaboralId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("empleado_turno_laboral_turno_laboral_id_fkey"),
                    l => l.HasOne<Empleado>().WithMany()
                        .HasForeignKey("EmpleadoId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("empleado_turno_laboral_empleado_id_fkey"),
                    j =>
                    {
                        j.HasKey("EmpleadoId", "TurnoLaboralId").HasName("empleado_turno_laboral_pkey");
                        j.ToTable("empleado_turno_laboral");
                        j.IndexerProperty<int>("EmpleadoId").HasColumnName("empleado_id");
                        j.IndexerProperty<int>("TurnoLaboralId").HasColumnName("turno_laboral_id");
                    });
        });

        modelBuilder.Entity<EstadoComandum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("estado_comanda_pkey");

            entity.ToTable("estado_comanda");

            entity.HasIndex(e => e.Descripcion, "estado_comanda_descripcion_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
        });

        modelBuilder.Entity<EstadoMesa>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("estado_mesa_pkey");

            entity.ToTable("estado_mesa");

            entity.HasIndex(e => e.Descripcion, "estado_mesa_descripcion_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
        });

        modelBuilder.Entity<EstadoPedido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("estado_pedido_pkey");

            entity.ToTable("estado_pedido");

            entity.HasIndex(e => e.Descripcion, "estado_pedido_descripcion_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
        });

        modelBuilder.Entity<FilaVirtual>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("fila_virtual_pkey");

            entity.ToTable("fila_virtual");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Habilitada)
                .HasDefaultValue(true)
                .HasColumnName("habilitada");
            entity.Property(e => e.RestauranteId).HasColumnName("restaurante_id");

            entity.HasOne(d => d.Restaurante).WithMany(p => p.FilaVirtuals)
                .HasForeignKey(d => d.RestauranteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fila_virtual_restaurante_id_fkey");
        });

        modelBuilder.Entity<Gerente>(entity =>
        {
            entity.HasKey(e => e.IdEmpleado).HasName("gerente_pkey");

            entity.ToTable("gerente");

            entity.Property(e => e.IdEmpleado)
                .ValueGeneratedNever()
                .HasColumnName("id_empleado");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithOne(p => p.Gerente)
                .HasForeignKey<Gerente>(d => d.IdEmpleado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("gerente_id_empleado_fkey");
        });

        modelBuilder.Entity<Grilla>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("grilla_pkey");

            entity.ToTable("grilla");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CantColumnas).HasColumnName("cant_columnas");
            entity.Property(e => e.CantFilas).HasColumnName("cant_filas");
            entity.Property(e => e.RestauranteId).HasColumnName("restaurante_id");

            entity.HasOne(d => d.Restaurante).WithMany(p => p.Grillas)
                .HasForeignKey(d => d.RestauranteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("grilla_restaurante_id_fkey");
        });

        modelBuilder.Entity<Ingrediente>(entity =>
        {
            entity.HasKey(e => e.IdInsumo).HasName("ingrediente_pkey");

            entity.ToTable("ingrediente");

            entity.Property(e => e.IdInsumo)
                .ValueGeneratedNever()
                .HasColumnName("id_insumo");

            entity.HasOne(d => d.IdInsumoNavigation).WithOne(p => p.Ingrediente)
                .HasForeignKey<Ingrediente>(d => d.IdInsumo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ingrediente_id_insumo_fkey");
        });

        modelBuilder.Entity<IngredienteIngredientePreparado>(entity =>
        {
            entity.HasKey(e => new { e.IngredienteId, e.IngredientePreparadoId }).HasName("ingrediente_ingrediente_preparado_pkey");

            entity.ToTable("ingrediente_ingrediente_preparado");

            entity.Property(e => e.IngredienteId).HasColumnName("ingrediente_id");
            entity.Property(e => e.IngredientePreparadoId).HasColumnName("ingrediente_preparado_id");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");

            entity.HasOne(d => d.Ingrediente).WithMany(p => p.IngredienteIngredientePreparados)
                .HasForeignKey(d => d.IngredienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ingrediente_ingrediente_preparado_ingrediente_id_fkey");

            entity.HasOne(d => d.IngredientePreparado).WithMany(p => p.IngredienteIngredientePreparados)
                .HasForeignKey(d => d.IngredientePreparadoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ingrediente_ingrediente_preparado_ingrediente_preparado_id_fkey");
        });

        modelBuilder.Entity<IngredientePreparado>(entity =>
        {
            entity.HasKey(e => e.IdIngrediente).HasName("ingrediente_preparado_pkey");

            entity.ToTable("ingrediente_preparado");

            entity.Property(e => e.IdIngrediente)
                .ValueGeneratedNever()
                .HasColumnName("id_ingrediente");

            entity.HasOne(d => d.IdIngredienteNavigation).WithOne(p => p.IngredientePreparado)
                .HasForeignKey<IngredientePreparado>(d => d.IdIngrediente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ingrediente_preparado_id_ingrediente_fkey");
        });

        modelBuilder.Entity<Insumo>(entity =>
        {
            entity.HasKey(e => e.IdArticulo).HasName("insumo_pkey");

            entity.ToTable("insumo");

            entity.Property(e => e.IdArticulo)
                .ValueGeneratedNever()
                .HasColumnName("id_articulo");
            entity.Property(e => e.CategoriaInsumoId).HasColumnName("categoria_insumo_id");
            entity.Property(e => e.StockMinimo).HasColumnName("stock_minimo");
            entity.Property(e => e.UnidadMedidaId).HasColumnName("unidad_medida_id");

            entity.HasOne(d => d.CategoriaInsumo).WithMany(p => p.Insumos)
                .HasForeignKey(d => d.CategoriaInsumoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("insumo_categoria_insumo_id_fkey");

            entity.HasOne(d => d.IdArticuloNavigation).WithOne(p => p.Insumo)
                .HasForeignKey<Insumo>(d => d.IdArticulo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("insumo_id_articulo_fkey");

            entity.HasOne(d => d.UnidadMedida).WithMany(p => p.Insumos)
                .HasForeignKey(d => d.UnidadMedidaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("insumo_unidad_medida_id_fkey");
        });

        modelBuilder.Entity<Llamado>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("llamado_pkey");

            entity.ToTable("llamado");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoriaLlamadoId).HasColumnName("categoria_llamado_id");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.GerenteId).HasColumnName("gerente_id");
            entity.Property(e => e.MesaId).HasColumnName("mesa_id");
            entity.Property(e => e.MozoId).HasColumnName("mozo_id");
            entity.Property(e => e.Resuelto)
                .HasDefaultValue(false)
                .HasColumnName("resuelto");

            entity.HasOne(d => d.CategoriaLlamado).WithMany(p => p.Llamados)
                .HasForeignKey(d => d.CategoriaLlamadoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("llamado_categoria_llamado_id_fkey");

            entity.HasOne(d => d.Gerente).WithMany(p => p.Llamados)
                .HasForeignKey(d => d.GerenteId)
                .HasConstraintName("llamado_gerente_id_fkey");

            entity.HasOne(d => d.Mesa).WithMany(p => p.Llamados)
                .HasForeignKey(d => d.MesaId)
                .HasConstraintName("llamado_mesa_id_fkey");

            entity.HasOne(d => d.Mozo).WithMany(p => p.Llamados)
                .HasForeignKey(d => d.MozoId)
                .HasConstraintName("llamado_mozo_id_fkey");
        });

        modelBuilder.Entity<Lote>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("lote_pkey");

            entity.ToTable("lote");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BodegaId).HasColumnName("bodega_id");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.FechaAdquisicion).HasColumnName("fecha_adquisicion");
            entity.Property(e => e.FechaVencimiento).HasColumnName("fecha_vencimiento");
            entity.Property(e => e.InsumoId).HasColumnName("insumo_id");
            entity.Property(e => e.Nombre).HasColumnName("nombre");

            entity.HasOne(d => d.Bodega).WithMany(p => p.Lotes)
                .HasForeignKey(d => d.BodegaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("lote_bodega_id_fkey");

            entity.HasOne(d => d.Insumo).WithMany(p => p.Lotes)
                .HasForeignKey(d => d.InsumoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("lote_insumo_id_fkey");
        });

        modelBuilder.Entity<Mesa>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("mesa_pkey");

            entity.ToTable("mesa");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CantPersonasMax).HasColumnName("cant_personas_max");
            entity.Property(e => e.CodigoInvitacion).HasColumnName("codigo_invitacion");
            entity.Property(e => e.DimensionMesaId).HasColumnName("dimension_mesa_id");
            entity.Property(e => e.EstadoMesaId).HasColumnName("estado_mesa_id");
            entity.Property(e => e.GrillaId).HasColumnName("grilla_id");
            entity.Property(e => e.Numero).HasColumnName("numero");
            entity.Property(e => e.PosicionXFin).HasColumnName("posicion_x_fin");
            entity.Property(e => e.PosicionXInicio).HasColumnName("posicion_x_inicio");
            entity.Property(e => e.PosicionYFin).HasColumnName("posicion_y_fin");
            entity.Property(e => e.PosicionYInicio).HasColumnName("posicion_y_inicio");

            entity.HasOne(d => d.DimensionMesa).WithMany(p => p.Mesas)
                .HasForeignKey(d => d.DimensionMesaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("mesa_dimension_mesa_id_fkey");

            entity.HasOne(d => d.EstadoMesa).WithMany(p => p.Mesas)
                .HasForeignKey(d => d.EstadoMesaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("mesa_estado_mesa_id_fkey");

            entity.HasOne(d => d.Grilla).WithMany(p => p.Mesas)
                .HasForeignKey(d => d.GrillaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("mesa_grilla_id_fkey");
        });

        modelBuilder.Entity<MetodoDePago>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("metodo_de_pago_pkey");

            entity.ToTable("metodo_de_pago");

            entity.HasIndex(e => e.Descripcion, "metodo_de_pago_descripcion_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
        });

        modelBuilder.Entity<MetodoDePagoRestaurante>(entity =>
        {
            entity.HasKey(e => new { e.RestauranteId, e.MetodoDePagoId }).HasName("metodo_de_pago_restaurante_pkey");

            entity.ToTable("metodo_de_pago_restaurante");

            entity.Property(e => e.RestauranteId).HasColumnName("restaurante_id");
            entity.Property(e => e.MetodoDePagoId).HasColumnName("metodo_de_pago_id");
            entity.Property(e => e.Habilitado)
                .HasDefaultValue(true)
                .HasColumnName("habilitado");

            entity.HasOne(d => d.MetodoDePago).WithMany(p => p.MetodoDePagoRestaurantes)
                .HasForeignKey(d => d.MetodoDePagoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("metodo_de_pago_restaurante_metodo_de_pago_id_fkey");

            entity.HasOne(d => d.Restaurante).WithMany(p => p.MetodoDePagoRestaurantes)
                .HasForeignKey(d => d.RestauranteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("metodo_de_pago_restaurante_restaurante_id_fkey");
        });

        modelBuilder.Entity<Mozo>(entity =>
        {
            entity.HasKey(e => e.IdEmpleado).HasName("mozo_pkey");

            entity.ToTable("mozo");

            entity.Property(e => e.IdEmpleado)
                .ValueGeneratedNever()
                .HasColumnName("id_empleado");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithOne(p => p.Mozo)
                .HasForeignKey<Mozo>(d => d.IdEmpleado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("mozo_id_empleado_fkey");

            entity.HasMany(d => d.Mesas).WithMany(p => p.Mozos)
                .UsingEntity<Dictionary<string, object>>(
                    "MozoMesa",
                    r => r.HasOne<Mesa>().WithMany()
                        .HasForeignKey("MesaId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("mozo_mesa_mesa_id_fkey"),
                    l => l.HasOne<Mozo>().WithMany()
                        .HasForeignKey("MozoId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("mozo_mesa_mozo_id_fkey"),
                    j =>
                    {
                        j.HasKey("MozoId", "MesaId").HasName("mozo_mesa_pkey");
                        j.ToTable("mozo_mesa");
                        j.IndexerProperty<int>("MozoId").HasColumnName("mozo_id");
                        j.IndexerProperty<int>("MesaId").HasColumnName("mesa_id");
                    });
        });

        modelBuilder.Entity<Notificacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("notificacion_pkey");

            entity.ToTable("notificacion");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fecha");
            entity.Property(e => e.RestauranteId).HasColumnName("restaurante_id");
            entity.Property(e => e.Resuelta)
                .HasDefaultValue(false)
                .HasColumnName("resuelta");

            entity.HasOne(d => d.Restaurante).WithMany(p => p.Notificacions)
                .HasForeignKey(d => d.RestauranteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("notificacion_restaurante_id_fkey");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pago_pkey");

            entity.ToTable("pago");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CierreId).HasColumnName("cierre_id");
            entity.Property(e => e.MetodoPagoId).HasColumnName("metodo_pago_id");
            entity.Property(e => e.Total).HasColumnName("total");

            entity.HasOne(d => d.Cierre).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.CierreId)
                .HasConstraintName("pago_cierre_id_fkey");

            entity.HasOne(d => d.MetodoPago).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.MetodoPagoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pago_metodo_pago_id_fkey");
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pedido_pkey");

            entity.ToTable("pedido");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EstadoPedidoId).HasColumnName("estado_pedido_id");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("fecha");
            entity.Property(e => e.ProveedorId).HasColumnName("proveedor_id");

            entity.HasOne(d => d.EstadoPedido).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.EstadoPedidoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pedido_estado_pedido_id_fkey");

            entity.HasOne(d => d.Proveedor).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.ProveedorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pedido_proveedor_id_fkey");
        });

        modelBuilder.Entity<PedidoInsumo>(entity =>
        {
            entity.HasKey(e => new { e.PedidoId, e.InsumoId }).HasName("pedido_insumo_pkey");

            entity.ToTable("pedido_insumo");

            entity.Property(e => e.PedidoId).HasColumnName("pedido_id");
            entity.Property(e => e.InsumoId).HasColumnName("insumo_id");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.PrecioCompra).HasColumnName("precio_compra");

            entity.HasOne(d => d.Insumo).WithMany(p => p.PedidoInsumos)
                .HasForeignKey(d => d.InsumoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pedido_insumo_insumo_id_fkey");

            entity.HasOne(d => d.Pedido).WithMany(p => p.PedidoInsumos)
                .HasForeignKey(d => d.PedidoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pedido_insumo_pedido_id_fkey");
        });

        modelBuilder.Entity<Plato>(entity =>
        {
            entity.HasKey(e => e.IdArticulo).HasName("plato_pkey");

            entity.ToTable("plato");

            entity.Property(e => e.IdArticulo)
                .ValueGeneratedNever()
                .HasColumnName("id_articulo");
            entity.Property(e => e.CategoriaPlatoId).HasColumnName("categoria_plato_id");
            entity.Property(e => e.Destacado)
                .HasDefaultValue(false)
                .HasColumnName("destacado");
            entity.Property(e => e.Sugerencia)
                .HasDefaultValue(false)
                .HasColumnName("sugerencia");
            entity.Property(e => e.TiempoPreparacionBase).HasColumnName("tiempo_preparacion_base");
            entity.Property(e => e.TipoPlatoId).HasColumnName("tipo_plato_id");

            entity.HasOne(d => d.CategoriaPlato).WithMany(p => p.Platos)
                .HasForeignKey(d => d.CategoriaPlatoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("plato_categoria_plato_id_fkey");

            entity.HasOne(d => d.IdArticuloNavigation).WithOne(p => p.Plato)
                .HasForeignKey<Plato>(d => d.IdArticulo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("plato_id_articulo_fkey");

            entity.HasOne(d => d.TipoPlato).WithMany(p => p.Platos)
                .HasForeignKey(d => d.TipoPlatoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("plato_tipo_plato_id_fkey");
        });

        modelBuilder.Entity<PlatoIngrediente>(entity =>
        {
            entity.HasKey(e => new { e.PlatoId, e.IngredienteId }).HasName("plato_ingrediente_pkey");

            entity.ToTable("plato_ingrediente");

            entity.Property(e => e.PlatoId).HasColumnName("plato_id");
            entity.Property(e => e.IngredienteId).HasColumnName("ingrediente_id");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.Opcional)
                .HasDefaultValue(false)
                .HasColumnName("opcional");

            entity.HasOne(d => d.Ingrediente).WithMany(p => p.PlatoIngredientes)
                .HasForeignKey(d => d.IngredienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("plato_ingrediente_ingrediente_id_fkey");

            entity.HasOne(d => d.Plato).WithMany(p => p.PlatoIngredientes)
                .HasForeignKey(d => d.PlatoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("plato_ingrediente_plato_id_fkey");
        });

        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("proveedor_pkey");

            entity.ToTable("proveedor");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Eliminado)
                .HasDefaultValue(false)
                .HasColumnName("eliminado");
            entity.Property(e => e.Nombre).HasColumnName("nombre");
            entity.Property(e => e.NumeroTelefonoWsp).HasColumnName("numero_telefono_wsp");
            entity.Property(e => e.RestauranteId).HasColumnName("restaurante_id");

            entity.HasOne(d => d.Restaurante).WithMany(p => p.Proveedors)
                .HasForeignKey(d => d.RestauranteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("proveedor_restaurante_id_fkey");
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("reserva_pkey");

            entity.ToTable("reserva");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CantComensales).HasColumnName("cant_comensales");
            entity.Property(e => e.Fecha).HasColumnName("fecha");
            entity.Property(e => e.Horario).HasColumnName("horario");
            entity.Property(e => e.MesaId).HasColumnName("mesa_id");
            entity.Property(e => e.NombreTitular).HasColumnName("nombre_titular");
            entity.Property(e => e.TelContacto).HasColumnName("tel_contacto");

            entity.HasOne(d => d.Mesa).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.MesaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reserva_mesa_id_fkey");
        });

        modelBuilder.Entity<Restaurante>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("restaurante_pkey");

            entity.ToTable("restaurante");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ColorPrincipal).HasColumnName("color_principal");
            entity.Property(e => e.ColorSecundario).HasColumnName("color_secundario");
            entity.Property(e => e.DireccionId).HasColumnName("direccion_id");
            entity.Property(e => e.Imagen).HasColumnName("imagen");
            entity.Property(e => e.Nombre).HasColumnName("nombre");
            entity.Property(e => e.TextoPrincipal).HasColumnName("texto_principal");
            entity.Property(e => e.TextoSecundario).HasColumnName("texto_secundario");

            entity.HasOne(d => d.Direccion).WithMany(p => p.Restaurantes)
                .HasForeignKey(d => d.DireccionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("restaurante_direccion_id_fkey");
        });

        modelBuilder.Entity<Restriccion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("restriccion_pkey");

            entity.ToTable("restriccion");

            entity.HasIndex(e => e.Descripcion, "restriccion_descripcion_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");

            entity.HasMany(d => d.Platos).WithMany(p => p.Restriccions)
                .UsingEntity<Dictionary<string, object>>(
                    "RestriccionPlato",
                    r => r.HasOne<Plato>().WithMany()
                        .HasForeignKey("PlatoId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("restriccion_plato_plato_id_fkey"),
                    l => l.HasOne<Restriccion>().WithMany()
                        .HasForeignKey("RestriccionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("restriccion_plato_restriccion_id_fkey"),
                    j =>
                    {
                        j.HasKey("RestriccionId", "PlatoId").HasName("restriccion_plato_pkey");
                        j.ToTable("restriccion_plato");
                        j.IndexerProperty<int>("RestriccionId").HasColumnName("restriccion_id");
                        j.IndexerProperty<int>("PlatoId").HasColumnName("plato_id");
                    });
        });

        modelBuilder.Entity<SugerenciaPlatoIum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sugerencia_plato_ia_pkey");

            entity.ToTable("sugerencia_plato_ia");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Json)
                .HasColumnType("jsonb")
                .HasColumnName("json");
            entity.Property(e => e.RestauranteId).HasColumnName("restaurante_id");

            entity.HasOne(d => d.Restaurante).WithMany(p => p.SugerenciaPlatoIa)
                .HasForeignKey(d => d.RestauranteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sugerencia_plato_ia_restaurante_id_fkey");
        });

        modelBuilder.Entity<TipoBodega>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tipo_bodega_pkey");

            entity.ToTable("tipo_bodega");

            entity.HasIndex(e => e.Descripcion, "tipo_bodega_descripcion_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
        });

        modelBuilder.Entity<TipoPlato>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tipo_plato_pkey");

            entity.ToTable("tipo_plato");

            entity.HasIndex(e => e.Descripcion, "tipo_plato_descripcion_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
        });

        modelBuilder.Entity<TurnoFila>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("turno_fila_pkey");

            entity.ToTable("turno_fila");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FilaVirtualId).HasColumnName("fila_virtual_id");
            entity.Property(e => e.Numero).HasColumnName("numero");

            entity.HasOne(d => d.FilaVirtual).WithMany(p => p.TurnoFilas)
                .HasForeignKey(d => d.FilaVirtualId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("turno_fila_fila_virtual_id_fkey");
        });

        modelBuilder.Entity<TurnoLaboral>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("turno_laboral_pkey");

            entity.ToTable("turno_laboral");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.HorarioLaboralFin).HasColumnName("horario_laboral_fin");
            entity.Property(e => e.HorarioLaboralInicio).HasColumnName("horario_laboral_inicio");
            entity.Property(e => e.RestauranteId).HasColumnName("restaurante_id");

            entity.HasOne(d => d.Restaurante).WithMany(p => p.TurnoLaborals)
                .HasForeignKey(d => d.RestauranteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("turno_laboral_restaurante_id_fkey");
        });

        modelBuilder.Entity<Ubicacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ubicacion_pkey");

            entity.ToTable("ubicacion");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Ciudad).HasColumnName("ciudad");
            entity.Property(e => e.CodigoPostal).HasColumnName("codigo_postal");
            entity.Property(e => e.Direccion).HasColumnName("direccion");
        });

        modelBuilder.Entity<UnidadMedidum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("unidad_medida_pkey");

            entity.ToTable("unidad_medida");

            entity.HasIndex(e => e.Nombre, "unidad_medida_nombre_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nombre).HasColumnName("nombre");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
