using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PanComido.Infraestructura.Persistencia.Entidades;

namespace PanComido.Infraestructura.Persistencia;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Articulo> Articulos { get; set; }

    public virtual DbSet<ArticuloComandaIngredienteExcluido> ArticuloComandaIngredienteExcluidos { get; set; }

    public virtual DbSet<ArticuloComandum> ArticuloComanda { get; set; }

    public virtual DbSet<BebidaPreparadaInsumo> BebidaPreparadaInsumos { get; set; }

    public virtual DbSet<BebidaPreparadum> BebidaPreparada { get; set; }

    public virtual DbSet<Bodega> Bodegas { get; set; }

    public virtual DbSet<Cartum> Carta { get; set; }

    public virtual DbSet<CategoriaInsumo> CategoriaInsumos { get; set; }

    public virtual DbSet<CategoriaLlamado> CategoriaLlamados { get; set; }

    public virtual DbSet<CategoriaPlato> CategoriaPlatos { get; set; }

    public virtual DbSet<Cierre> Cierres { get; set; }

    public virtual DbSet<Cocina> Cocinas { get; set; }

    public virtual DbSet<Comandum> Comanda { get; set; }

    public virtual DbSet<ConfiguracionArticulo> ConfiguracionArticulos { get; set; }

    public virtual DbSet<DatosTransferencium> DatosTransferencia { get; set; }

    public virtual DbSet<DimensionMesa> DimensionMesas { get; set; }

    public virtual DbSet<Empleado> Empleados { get; set; }

    public virtual DbSet<EncuestaSatisfaccion> EncuestaSatisfaccions { get; set; }

    public virtual DbSet<EstadoComandum> EstadoComanda { get; set; }

    public virtual DbSet<EstadoMesa> EstadoMesas { get; set; }

    public virtual DbSet<EstadoPago> EstadoPagos { get; set; }

    public virtual DbSet<EstadoPedido> EstadoPedidos { get; set; }

    public virtual DbSet<FamiliaTipografica> FamiliaTipograficas { get; set; }

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

    public virtual DbSet<PorcentajeCategoriaBebidum> PorcentajeCategoriaBebida { get; set; }

    public virtual DbSet<PorcentajeCategoriaPlato> PorcentajeCategoriaPlatos { get; set; }

    public virtual DbSet<Proveedor> Proveedors { get; set; }

    public virtual DbSet<ReglaTiempoExtra> ReglaTiempoExtras { get; set; }

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

            entity.Property(e => e.Eliminado).HasDefaultValue(false);
            entity.Property(e => e.EsPrecioManual).HasDefaultValue(true);

            entity.HasOne(d => d.Carta).WithMany(p => p.Articulos).HasConstraintName("articulo_carta_id_fkey");

            entity.HasOne(d => d.Restaurante).WithMany(p => p.Articulos)
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

        modelBuilder.Entity<ArticuloComandaIngredienteExcluido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("articulo_comanda_ingrediente_excluido_pkey");

            entity.HasOne(d => d.ArticuloComanda).WithMany(p => p.ArticuloComandaIngredienteExcluidos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("articulo_comanda_ingrediente_excluido_articulo_comanda_id_fkey");

            entity.HasOne(d => d.Ingrediente).WithMany(p => p.ArticuloComandaIngredienteExcluidos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("articulo_comanda_ingrediente_excluido_ingrediente_id_fkey");
        });

        modelBuilder.Entity<ArticuloComandum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("articulo_comanda_pkey");

            entity.Property(e => e.Cantidad).HasDefaultValue(1);
            entity.Property(e => e.Entregado).HasDefaultValue(false);
            entity.Property(e => e.NombreComensal).HasDefaultValueSql("'Anónimo'::text");

            entity.HasOne(d => d.Articulo).WithMany(p => p.ArticuloComanda)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("articulo_comanda_articulo_id_fkey");

            entity.HasOne(d => d.Comanda).WithMany(p => p.ArticuloComanda)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("articulo_comanda_comanda_id_fkey");
        });

        modelBuilder.Entity<BebidaPreparadaInsumo>(entity =>
        {
            entity.HasKey(e => new { e.BebidaPreparadaId, e.InsumoId }).HasName("bebida_preparada_insumo_pkey");

            entity.HasOne(d => d.BebidaPreparada).WithMany(p => p.BebidaPreparadaInsumos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bebida_preparada_insumo_bebida_preparada_id_fkey");

            entity.HasOne(d => d.Insumo).WithMany(p => p.BebidaPreparadaInsumos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bebida_preparada_insumo_insumo_id_fkey");
        });

        modelBuilder.Entity<BebidaPreparadum>(entity =>
        {
            entity.HasKey(e => e.IdArticulo).HasName("bebida_preparada_pkey");

            entity.Property(e => e.IdArticulo).ValueGeneratedNever();

            entity.HasOne(d => d.IdArticuloNavigation).WithOne(p => p.BebidaPreparadum)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bebida_preparada_id_articulo_fkey");
        });

        modelBuilder.Entity<Bodega>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("bodega_pkey");

            entity.Property(e => e.Eliminado).HasDefaultValue(false);

            entity.HasOne(d => d.Restaurante).WithMany(p => p.Bodegas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bodega_restaurante_id_fkey");

            entity.HasOne(d => d.TipoBodega).WithMany(p => p.Bodegas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bodega_tipo_bodega_id_fkey");
        });

        modelBuilder.Entity<Cartum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("carta_pkey");

            entity.HasOne(d => d.Restaurante).WithMany(p => p.Carta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("carta_restaurante_id_fkey");
        });

        modelBuilder.Entity<CategoriaInsumo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("categoria_insumo_pkey");

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
        });

        modelBuilder.Entity<CategoriaPlato>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("categoria_plato_pkey");
        });

        modelBuilder.Entity<Cierre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cierre_pkey");

            entity.Property(e => e.Fecha).HasDefaultValueSql("CURRENT_DATE");

            entity.HasOne(d => d.Restaurante).WithMany(p => p.Cierres)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cierre_restaurante_id_fkey");

            entity.HasOne(d => d.TurnoLaboral).WithMany(p => p.Cierres)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cierre_turno_laboral_id_fkey");
        });

        modelBuilder.Entity<Cocina>(entity =>
        {
            entity.HasKey(e => e.IdEmpleado).HasName("cocina_pkey");

            entity.Property(e => e.IdEmpleado).ValueGeneratedNever();

            entity.HasOne(d => d.IdEmpleadoNavigation).WithOne(p => p.Cocina)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cocina_id_empleado_fkey");
        });

        modelBuilder.Entity<Comandum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("comanda_pkey");

            entity.Property(e => e.HoraInicio).HasDefaultValueSql("now()");
            entity.Property(e => e.HoraUltimoCambioEstado).HasDefaultValueSql("now()");

            entity.HasOne(d => d.EstadoComanda).WithMany(p => p.Comanda)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("comanda_estado_comanda_id_fkey");

            entity.HasOne(d => d.Mesa).WithMany(p => p.Comanda)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("comanda_mesa_id_fkey");

            entity.HasOne(d => d.Restaurante).WithMany(p => p.Comanda)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("comanda_restaurante_id_fkey");
        });

        modelBuilder.Entity<ConfiguracionArticulo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("configuracion_articulo_pkey");
        });

        modelBuilder.Entity<DatosTransferencium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("datos_transferencia_pkey");

            entity.HasOne(d => d.Restaurante).WithOne(p => p.DatosTransferencium)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("datos_transferencia_restaurante_id_fkey");
        });

        modelBuilder.Entity<DimensionMesa>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("dimension_mesa_pkey");
        });

        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("empleado_pkey");

            entity.Property(e => e.Eliminado).HasDefaultValue(false);
            entity.Property(e => e.Estado).HasDefaultValueSql("'activo'::text");

            entity.HasOne(d => d.Restaurante).WithMany(p => p.Empleados)
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

        modelBuilder.Entity<EncuestaSatisfaccion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("encuesta_satisfaccion_pkey");

            entity.Property(e => e.Fecha).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Comanda).WithMany(p => p.EncuestaSatisfaccions).HasConstraintName("encuesta_satisfaccion_comanda_id_fkey");
        });

        modelBuilder.Entity<EstadoComandum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("estado_comanda_pkey");
        });

        modelBuilder.Entity<EstadoMesa>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("estado_mesa_pkey");
        });

        modelBuilder.Entity<EstadoPago>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("estado_pago_pkey");
        });

        modelBuilder.Entity<EstadoPedido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("estado_pedido_pkey");
        });

        modelBuilder.Entity<FamiliaTipografica>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("familia_tipografica_pkey");
        });

        modelBuilder.Entity<FilaVirtual>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("fila_virtual_pkey");

            entity.Property(e => e.Habilitada).HasDefaultValue(true);

            entity.HasOne(d => d.Restaurante).WithMany(p => p.FilaVirtuals)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fila_virtual_restaurante_id_fkey");
        });

        modelBuilder.Entity<Gerente>(entity =>
        {
            entity.HasKey(e => e.IdEmpleado).HasName("gerente_pkey");

            entity.Property(e => e.IdEmpleado).ValueGeneratedNever();

            entity.HasOne(d => d.IdEmpleadoNavigation).WithOne(p => p.Gerente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("gerente_id_empleado_fkey");
        });

        modelBuilder.Entity<Grilla>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("grilla_pkey");

            entity.HasOne(d => d.Restaurante).WithMany(p => p.Grillas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("grilla_restaurante_id_fkey");
        });

        modelBuilder.Entity<Ingrediente>(entity =>
        {
            entity.HasKey(e => e.IdInsumo).HasName("ingrediente_pkey");

            entity.Property(e => e.IdInsumo).ValueGeneratedNever();

            entity.HasOne(d => d.IdInsumoNavigation).WithOne(p => p.Ingrediente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ingrediente_id_insumo_fkey");
        });

        modelBuilder.Entity<IngredienteIngredientePreparado>(entity =>
        {
            entity.HasKey(e => new { e.IngredienteId, e.IngredientePreparadoId }).HasName("ingrediente_ingrediente_preparado_pkey");

            entity.HasOne(d => d.Ingrediente).WithMany(p => p.IngredienteIngredientePreparados)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ingrediente_ingrediente_preparado_ingrediente_id_fkey");

            entity.HasOne(d => d.IngredientePreparado).WithMany(p => p.IngredienteIngredientePreparados)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ingrediente_ingrediente_preparado_ingrediente_preparado_id_fkey");
        });

        modelBuilder.Entity<IngredientePreparado>(entity =>
        {
            entity.HasKey(e => e.IdIngrediente).HasName("ingrediente_preparado_pkey");

            entity.Property(e => e.IdIngrediente).ValueGeneratedNever();

            entity.HasOne(d => d.IdIngredienteNavigation).WithOne(p => p.IngredientePreparado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ingrediente_preparado_id_ingrediente_fkey");
        });

        modelBuilder.Entity<Insumo>(entity =>
        {
            entity.HasKey(e => e.IdArticulo).HasName("insumo_pkey");

            entity.Property(e => e.IdArticulo).ValueGeneratedNever();

            entity.HasOne(d => d.CategoriaInsumo).WithMany(p => p.Insumos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("insumo_categoria_insumo_id_fkey");

            entity.HasOne(d => d.IdArticuloNavigation).WithOne(p => p.Insumo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("insumo_id_articulo_fkey");

            entity.HasOne(d => d.UnidadMedida).WithMany(p => p.Insumos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("insumo_unidad_medida_id_fkey");
        });

        modelBuilder.Entity<Llamado>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("llamado_pkey");

            entity.Property(e => e.Resuelto).HasDefaultValue(false);

            entity.HasOne(d => d.CategoriaLlamado).WithMany(p => p.Llamados)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("llamado_categoria_llamado_id_fkey");

            entity.HasOne(d => d.Gerente).WithMany(p => p.Llamados).HasConstraintName("llamado_gerente_id_fkey");

            entity.HasOne(d => d.Mesa).WithMany(p => p.Llamados).HasConstraintName("llamado_mesa_id_fkey");

            entity.HasOne(d => d.Mozo).WithMany(p => p.Llamados).HasConstraintName("llamado_mozo_id_fkey");
        });

        modelBuilder.Entity<Lote>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("lote_pkey");

            entity.HasOne(d => d.Bodega).WithMany(p => p.Lotes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("lote_bodega_id_fkey");

            entity.HasOne(d => d.Insumo).WithMany(p => p.Lotes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("lote_insumo_id_fkey");
        });

        modelBuilder.Entity<Mesa>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("mesa_pkey");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.TipoElemento).HasDefaultValue(1);

            entity.HasOne(d => d.DimensionMesa).WithMany(p => p.Mesas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("mesa_dimension_mesa_id_fkey");

            entity.HasOne(d => d.EstadoMesa).WithMany(p => p.Mesas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("mesa_estado_mesa_id_fkey");

            entity.HasOne(d => d.Grilla).WithMany(p => p.Mesas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("mesa_grilla_id_fkey");
        });

        modelBuilder.Entity<MetodoDePago>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("metodo_de_pago_pkey");
        });

        modelBuilder.Entity<MetodoDePagoRestaurante>(entity =>
        {
            entity.HasKey(e => new { e.RestauranteId, e.MetodoDePagoId }).HasName("metodo_de_pago_restaurante_pkey");

            entity.Property(e => e.Habilitado).HasDefaultValue(true);

            entity.HasOne(d => d.MetodoDePago).WithMany(p => p.MetodoDePagoRestaurantes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("metodo_de_pago_restaurante_metodo_de_pago_id_fkey");

            entity.HasOne(d => d.Restaurante).WithMany(p => p.MetodoDePagoRestaurantes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("metodo_de_pago_restaurante_restaurante_id_fkey");
        });

        modelBuilder.Entity<Mozo>(entity =>
        {
            entity.HasKey(e => e.IdEmpleado).HasName("mozo_pkey");

            entity.Property(e => e.IdEmpleado).ValueGeneratedNever();
            entity.Property(e => e.Activo).HasDefaultValue(true);

            entity.HasOne(d => d.IdEmpleadoNavigation).WithOne(p => p.Mozo)
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

            entity.Property(e => e.Fecha).HasDefaultValueSql("now()");
            entity.Property(e => e.Resuelta).HasDefaultValue(false);

            entity.HasOne(d => d.Restaurante).WithMany(p => p.Notificacions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("notificacion_restaurante_id_fkey");
        });

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pago_pkey");

            entity.Property(e => e.FechaHora).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Cierre).WithMany(p => p.Pagos).HasConstraintName("pago_cierre_id_fkey");

            entity.HasOne(d => d.Comanda).WithMany(p => p.Pagos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pago_comanda_id_fkey");

            entity.HasOne(d => d.EstadoPago).WithMany(p => p.Pagos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pago_estado_pago_id_fkey");

            entity.HasOne(d => d.MetodoPago).WithMany(p => p.Pagos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pago_metodo_pago_id_fkey");
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pedido_pkey");

            entity.Property(e => e.Fecha).HasDefaultValueSql("CURRENT_DATE");

            entity.HasOne(d => d.EstadoPedido).WithMany(p => p.Pedidos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pedido_estado_pedido_id_fkey");

            entity.HasOne(d => d.Proveedor).WithMany(p => p.Pedidos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pedido_proveedor_id_fkey");
        });

        modelBuilder.Entity<PedidoInsumo>(entity =>
        {
            entity.HasKey(e => new { e.PedidoId, e.InsumoId }).HasName("pedido_insumo_pkey");

            entity.HasOne(d => d.Insumo).WithMany(p => p.PedidoInsumos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pedido_insumo_insumo_id_fkey");

            entity.HasOne(d => d.Pedido).WithMany(p => p.PedidoInsumos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pedido_insumo_pedido_id_fkey");
        });

        modelBuilder.Entity<Plato>(entity =>
        {
            entity.HasKey(e => e.IdArticulo).HasName("plato_pkey");

            entity.Property(e => e.IdArticulo).ValueGeneratedNever();
            entity.Property(e => e.Destacado).HasDefaultValue(false);
            entity.Property(e => e.Sugerencia).HasDefaultValue(false);

            entity.HasOne(d => d.CategoriaPlato).WithMany(p => p.Platos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("plato_categoria_plato_id_fkey");

            entity.HasOne(d => d.IdArticuloNavigation).WithOne(p => p.Plato)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("plato_id_articulo_fkey");

            entity.HasOne(d => d.TipoPlato).WithMany(p => p.Platos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("plato_tipo_plato_id_fkey");
        });

        modelBuilder.Entity<PlatoIngrediente>(entity =>
        {
            entity.HasKey(e => new { e.PlatoId, e.IngredienteId }).HasName("plato_ingrediente_pkey");

            entity.Property(e => e.Opcional).HasDefaultValue(false);

            entity.HasOne(d => d.Ingrediente).WithMany(p => p.PlatoIngredientes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("plato_ingrediente_ingrediente_id_fkey");

            entity.HasOne(d => d.Plato).WithMany(p => p.PlatoIngredientes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("plato_ingrediente_plato_id_fkey");
        });

        modelBuilder.Entity<PorcentajeCategoriaBebidum>(entity =>
        {
            entity.HasKey(e => new { e.RestauranteId, e.CategoriaInsumoId }).HasName("porcentaje_categoria_bebida_pkey");

            entity.Property(e => e.Porcentaje).HasDefaultValueSql("20");

            entity.HasOne(d => d.CategoriaInsumo).WithMany(p => p.PorcentajeCategoriaBebida)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("porcentaje_categoria_bebida_categoria_insumo_id_fkey");

            entity.HasOne(d => d.Restaurante).WithMany(p => p.PorcentajeCategoriaBebida)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("porcentaje_categoria_bebida_restaurante_id_fkey");
        });

        modelBuilder.Entity<PorcentajeCategoriaPlato>(entity =>
        {
            entity.HasKey(e => new { e.RestauranteId, e.CategoriaPlatoId }).HasName("porcentaje_categoria_plato_pkey");

            entity.Property(e => e.Porcentaje).HasDefaultValueSql("20");

            entity.HasOne(d => d.CategoriaPlato).WithMany(p => p.PorcentajeCategoriaPlatos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("porcentaje_categoria_plato_categoria_plato_id_fkey");

            entity.HasOne(d => d.Restaurante).WithMany(p => p.PorcentajeCategoriaPlatos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("porcentaje_categoria_plato_restaurante_id_fkey");
        });

        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("proveedor_pkey");

            entity.Property(e => e.Eliminado).HasDefaultValue(false);

            entity.HasOne(d => d.Restaurante).WithMany(p => p.Proveedors)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("proveedor_restaurante_id_fkey");
        });

        modelBuilder.Entity<ReglaTiempoExtra>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("regla_tiempo_extra_pkey");

            entity.HasOne(d => d.Restaurante).WithMany(p => p.ReglaTiempoExtras).HasConstraintName("regla_tiempo_extra_restaurante_id_fkey");
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("reserva_pkey");

            entity.HasOne(d => d.Mesa).WithMany(p => p.Reservas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reserva_mesa_id_fkey");
        });

        modelBuilder.Entity<Restaurante>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("restaurante_pkey");

            entity.HasOne(d => d.Direccion).WithMany(p => p.Restaurantes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("restaurante_direccion_id_fkey");

            entity.HasOne(d => d.FamiliaTipografica).WithMany(p => p.Restaurantes).HasConstraintName("restaurante_familia_tipografica_id_fkey");
        });

        modelBuilder.Entity<Restriccion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("restriccion_pkey");

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

            entity.HasOne(d => d.Restaurante).WithMany(p => p.SugerenciaPlatoIa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sugerencia_plato_ia_restaurante_id_fkey");
        });

        modelBuilder.Entity<TipoBodega>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tipo_bodega_pkey");
        });

        modelBuilder.Entity<TipoPlato>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tipo_plato_pkey");
        });

        modelBuilder.Entity<TurnoFila>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("turno_fila_pkey");

            entity.HasOne(d => d.FilaVirtual).WithMany(p => p.TurnoFilas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("turno_fila_fila_virtual_id_fkey");
        });

        modelBuilder.Entity<TurnoLaboral>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("turno_laboral_pkey");

            entity.Property(e => e.EsNocturno).HasDefaultValue(false);

            entity.HasOne(d => d.Restaurante).WithMany(p => p.TurnoLaborals)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("turno_laboral_restaurante_id_fkey");
        });

        modelBuilder.Entity<Ubicacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("ubicacion_pkey");
        });

        modelBuilder.Entity<UnidadMedidum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("unidad_medida_pkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
