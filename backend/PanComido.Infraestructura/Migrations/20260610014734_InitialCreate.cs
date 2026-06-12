using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PanComido.Infraestructura.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categoria_insumo",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descripcion = table.Column<string>(type: "text", nullable: false),
                    tipo_aplica = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("categoria_insumo_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "categoria_llamado",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descripcion = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("categoria_llamado_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "categoria_plato",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descripcion = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("categoria_plato_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "configuracion_articulo",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descripcion = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("configuracion_articulo_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "dimension_mesa",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    imagen = table.Column<string>(type: "text", nullable: true),
                    forma = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("dimension_mesa_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "estado_comanda",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descripcion = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("estado_comanda_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "estado_mesa",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descripcion = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("estado_mesa_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "estado_pedido",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descripcion = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("estado_pedido_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "MesaMozo",
                columns: table => new
                {
                    MesaId = table.Column<int>(type: "integer", nullable: false),
                    MozoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MesaMozo", x => new { x.MesaId, x.MozoId });
                });

            migrationBuilder.CreateTable(
                name: "metodo_de_pago",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descripcion = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("metodo_de_pago_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "PlatoRestriccion",
                columns: table => new
                {
                    PlatoId = table.Column<int>(type: "integer", nullable: false),
                    RestriccionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatoRestriccion", x => new { x.PlatoId, x.RestriccionId });
                });

            migrationBuilder.CreateTable(
                name: "restriccion",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descripcion = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("restriccion_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tipo_bodega",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descripcion = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tipo_bodega_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tipo_plato",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    descripcion = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tipo_plato_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ubicacion",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    direccion = table.Column<string>(type: "text", nullable: false),
                    ciudad = table.Column<string>(type: "text", nullable: false),
                    codigo_postal = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ubicacion_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "unidad_medida",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("unidad_medida_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "restaurante",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    direccion_id = table.Column<int>(type: "integer", nullable: false),
                    nombre = table.Column<string>(type: "text", nullable: false),
                    imagen = table.Column<string>(type: "text", nullable: true),
                    color_principal = table.Column<string>(type: "text", nullable: true),
                    color_secundario = table.Column<string>(type: "text", nullable: true),
                    texto_principal = table.Column<string>(type: "text", nullable: true),
                    texto_secundario = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("restaurante_pkey", x => x.id);
                    table.ForeignKey(
                        name: "restaurante_direccion_id_fkey",
                        column: x => x.direccion_id,
                        principalTable: "ubicacion",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "bodega",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    restaurante_id = table.Column<int>(type: "integer", nullable: false),
                    tipo_bodega_id = table.Column<int>(type: "integer", nullable: false),
                    nombre = table.Column<string>(type: "text", nullable: false),
                    eliminado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("bodega_pkey", x => x.id);
                    table.ForeignKey(
                        name: "bodega_restaurante_id_fkey",
                        column: x => x.restaurante_id,
                        principalTable: "restaurante",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "bodega_tipo_bodega_id_fkey",
                        column: x => x.tipo_bodega_id,
                        principalTable: "tipo_bodega",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "carta",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    restaurante_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("carta_pkey", x => x.id);
                    table.ForeignKey(
                        name: "carta_restaurante_id_fkey",
                        column: x => x.restaurante_id,
                        principalTable: "restaurante",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "empleado",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    restaurante_id = table.Column<int>(type: "integer", nullable: false),
                    nombre = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    contrasena = table.Column<string>(type: "text", nullable: false),
                    estado = table.Column<string>(type: "text", nullable: false, defaultValueSql: "'activo'::text"),
                    eliminado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("empleado_pkey", x => x.id);
                    table.ForeignKey(
                        name: "empleado_restaurante_id_fkey",
                        column: x => x.restaurante_id,
                        principalTable: "restaurante",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "fila_virtual",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    restaurante_id = table.Column<int>(type: "integer", nullable: false),
                    habilitada = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("fila_virtual_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fila_virtual_restaurante_id_fkey",
                        column: x => x.restaurante_id,
                        principalTable: "restaurante",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "grilla",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    restaurante_id = table.Column<int>(type: "integer", nullable: false),
                    cant_columnas = table.Column<int>(type: "integer", nullable: false),
                    cant_filas = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("grilla_pkey", x => x.id);
                    table.ForeignKey(
                        name: "grilla_restaurante_id_fkey",
                        column: x => x.restaurante_id,
                        principalTable: "restaurante",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "metodo_de_pago_restaurante",
                columns: table => new
                {
                    restaurante_id = table.Column<int>(type: "integer", nullable: false),
                    metodo_de_pago_id = table.Column<int>(type: "integer", nullable: false),
                    habilitado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("metodo_de_pago_restaurante_pkey", x => new { x.restaurante_id, x.metodo_de_pago_id });
                    table.ForeignKey(
                        name: "metodo_de_pago_restaurante_metodo_de_pago_id_fkey",
                        column: x => x.metodo_de_pago_id,
                        principalTable: "metodo_de_pago",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "metodo_de_pago_restaurante_restaurante_id_fkey",
                        column: x => x.restaurante_id,
                        principalTable: "restaurante",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "notificacion",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    restaurante_id = table.Column<int>(type: "integer", nullable: false),
                    fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    descripcion = table.Column<string>(type: "text", nullable: false),
                    resuelta = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("notificacion_pkey", x => x.id);
                    table.ForeignKey(
                        name: "notificacion_restaurante_id_fkey",
                        column: x => x.restaurante_id,
                        principalTable: "restaurante",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "proveedor",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    restaurante_id = table.Column<int>(type: "integer", nullable: false),
                    nombre = table.Column<string>(type: "text", nullable: false),
                    numero_telefono_wsp = table.Column<string>(type: "text", nullable: true),
                    eliminado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("proveedor_pkey", x => x.id);
                    table.ForeignKey(
                        name: "proveedor_restaurante_id_fkey",
                        column: x => x.restaurante_id,
                        principalTable: "restaurante",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "sugerencia_plato_ia",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    restaurante_id = table.Column<int>(type: "integer", nullable: false),
                    json = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("sugerencia_plato_ia_pkey", x => x.id);
                    table.ForeignKey(
                        name: "sugerencia_plato_ia_restaurante_id_fkey",
                        column: x => x.restaurante_id,
                        principalTable: "restaurante",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "turno_laboral",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    restaurante_id = table.Column<int>(type: "integer", nullable: false),
                    horario_laboral_inicio = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    horario_laboral_fin = table.Column<TimeOnly>(type: "time without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("turno_laboral_pkey", x => x.id);
                    table.ForeignKey(
                        name: "turno_laboral_restaurante_id_fkey",
                        column: x => x.restaurante_id,
                        principalTable: "restaurante",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "articulo",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    carta_id = table.Column<int>(type: "integer", nullable: true),
                    restaurante_id = table.Column<int>(type: "integer", nullable: false),
                    nombre = table.Column<string>(type: "text", nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    precio_venta_final = table.Column<decimal>(type: "numeric", nullable: true),
                    precio_ganancia = table.Column<decimal>(type: "numeric", nullable: true),
                    precio_promocional = table.Column<decimal>(type: "numeric", nullable: true),
                    url_imagen = table.Column<string>(type: "text", nullable: true),
                    eliminado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("articulo_pkey", x => x.id);
                    table.ForeignKey(
                        name: "articulo_carta_id_fkey",
                        column: x => x.carta_id,
                        principalTable: "carta",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "articulo_restaurante_id_fkey",
                        column: x => x.restaurante_id,
                        principalTable: "restaurante",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "cocina",
                columns: table => new
                {
                    id_empleado = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("cocina_pkey", x => x.id_empleado);
                    table.ForeignKey(
                        name: "cocina_id_empleado_fkey",
                        column: x => x.id_empleado,
                        principalTable: "empleado",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "gerente",
                columns: table => new
                {
                    id_empleado = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("gerente_pkey", x => x.id_empleado);
                    table.ForeignKey(
                        name: "gerente_id_empleado_fkey",
                        column: x => x.id_empleado,
                        principalTable: "empleado",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "mozo",
                columns: table => new
                {
                    id_empleado = table.Column<int>(type: "integer", nullable: false),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("mozo_pkey", x => x.id_empleado);
                    table.ForeignKey(
                        name: "mozo_id_empleado_fkey",
                        column: x => x.id_empleado,
                        principalTable: "empleado",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "turno_fila",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fila_virtual_id = table.Column<int>(type: "integer", nullable: false),
                    numero = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("turno_fila_pkey", x => x.id);
                    table.ForeignKey(
                        name: "turno_fila_fila_virtual_id_fkey",
                        column: x => x.fila_virtual_id,
                        principalTable: "fila_virtual",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "mesa",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    grilla_id = table.Column<int>(type: "integer", nullable: false),
                    estado_mesa_id = table.Column<int>(type: "integer", nullable: false),
                    dimension_mesa_id = table.Column<int>(type: "integer", nullable: false),
                    posicion_x_inicio = table.Column<int>(type: "integer", nullable: false),
                    posicion_x_fin = table.Column<int>(type: "integer", nullable: false),
                    posicion_y_inicio = table.Column<int>(type: "integer", nullable: false),
                    posicion_y_fin = table.Column<int>(type: "integer", nullable: false),
                    numero = table.Column<int>(type: "integer", nullable: false),
                    codigo_invitacion = table.Column<string>(type: "text", nullable: true),
                    cant_personas_max = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("mesa_pkey", x => x.id);
                    table.ForeignKey(
                        name: "mesa_dimension_mesa_id_fkey",
                        column: x => x.dimension_mesa_id,
                        principalTable: "dimension_mesa",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "mesa_estado_mesa_id_fkey",
                        column: x => x.estado_mesa_id,
                        principalTable: "estado_mesa",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "mesa_grilla_id_fkey",
                        column: x => x.grilla_id,
                        principalTable: "grilla",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "categoria_insumo_proveedor",
                columns: table => new
                {
                    categoria_insumo_id = table.Column<int>(type: "integer", nullable: false),
                    proveedor_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("categoria_insumo_proveedor_pkey", x => new { x.categoria_insumo_id, x.proveedor_id });
                    table.ForeignKey(
                        name: "categoria_insumo_proveedor_categoria_insumo_id_fkey",
                        column: x => x.categoria_insumo_id,
                        principalTable: "categoria_insumo",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "categoria_insumo_proveedor_proveedor_id_fkey",
                        column: x => x.proveedor_id,
                        principalTable: "proveedor",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "pedido",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    proveedor_id = table.Column<int>(type: "integer", nullable: false),
                    estado_pedido_id = table.Column<int>(type: "integer", nullable: false),
                    fecha = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "CURRENT_DATE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pedido_pkey", x => x.id);
                    table.ForeignKey(
                        name: "pedido_estado_pedido_id_fkey",
                        column: x => x.estado_pedido_id,
                        principalTable: "estado_pedido",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "pedido_proveedor_id_fkey",
                        column: x => x.proveedor_id,
                        principalTable: "proveedor",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "cierre",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    restaurante_id = table.Column<int>(type: "integer", nullable: false),
                    turno_laboral_id = table.Column<int>(type: "integer", nullable: false),
                    diferencia = table.Column<decimal>(type: "numeric", nullable: false),
                    sobrante = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("cierre_pkey", x => x.id);
                    table.ForeignKey(
                        name: "cierre_restaurante_id_fkey",
                        column: x => x.restaurante_id,
                        principalTable: "restaurante",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "cierre_turno_laboral_id_fkey",
                        column: x => x.turno_laboral_id,
                        principalTable: "turno_laboral",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "empleado_turno_laboral",
                columns: table => new
                {
                    empleado_id = table.Column<int>(type: "integer", nullable: false),
                    turno_laboral_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("empleado_turno_laboral_pkey", x => new { x.empleado_id, x.turno_laboral_id });
                    table.ForeignKey(
                        name: "empleado_turno_laboral_empleado_id_fkey",
                        column: x => x.empleado_id,
                        principalTable: "empleado",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "empleado_turno_laboral_turno_laboral_id_fkey",
                        column: x => x.turno_laboral_id,
                        principalTable: "turno_laboral",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "articulo_configuracion_articulo",
                columns: table => new
                {
                    articulo_id = table.Column<int>(type: "integer", nullable: false),
                    configuracion_articulo_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("articulo_configuracion_articulo_pkey", x => new { x.articulo_id, x.configuracion_articulo_id });
                    table.ForeignKey(
                        name: "articulo_configuracion_articulo_articulo_id_fkey",
                        column: x => x.articulo_id,
                        principalTable: "articulo",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "articulo_configuracion_articulo_configuracion_articulo_id_fkey",
                        column: x => x.configuracion_articulo_id,
                        principalTable: "configuracion_articulo",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "insumo",
                columns: table => new
                {
                    id_articulo = table.Column<int>(type: "integer", nullable: false),
                    categoria_insumo_id = table.Column<int>(type: "integer", nullable: false),
                    unidad_medida_id = table.Column<int>(type: "integer", nullable: false),
                    stock_minimo = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("insumo_pkey", x => x.id_articulo);
                    table.ForeignKey(
                        name: "insumo_categoria_insumo_id_fkey",
                        column: x => x.categoria_insumo_id,
                        principalTable: "categoria_insumo",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "insumo_id_articulo_fkey",
                        column: x => x.id_articulo,
                        principalTable: "articulo",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "insumo_unidad_medida_id_fkey",
                        column: x => x.unidad_medida_id,
                        principalTable: "unidad_medida",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "plato",
                columns: table => new
                {
                    id_articulo = table.Column<int>(type: "integer", nullable: false),
                    tipo_plato_id = table.Column<int>(type: "integer", nullable: false),
                    categoria_plato_id = table.Column<int>(type: "integer", nullable: false),
                    tiempo_preparacion_base = table.Column<int>(type: "integer", nullable: false),
                    destacado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    sugerencia = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("plato_pkey", x => x.id_articulo);
                    table.ForeignKey(
                        name: "plato_categoria_plato_id_fkey",
                        column: x => x.categoria_plato_id,
                        principalTable: "categoria_plato",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "plato_id_articulo_fkey",
                        column: x => x.id_articulo,
                        principalTable: "articulo",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "plato_tipo_plato_id_fkey",
                        column: x => x.tipo_plato_id,
                        principalTable: "tipo_plato",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "llamado",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    mozo_id = table.Column<int>(type: "integer", nullable: true),
                    gerente_id = table.Column<int>(type: "integer", nullable: true),
                    mesa_id = table.Column<int>(type: "integer", nullable: true),
                    categoria_llamado_id = table.Column<int>(type: "integer", nullable: false),
                    descripcion = table.Column<string>(type: "text", nullable: true),
                    resuelto = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("llamado_pkey", x => x.id);
                    table.ForeignKey(
                        name: "llamado_categoria_llamado_id_fkey",
                        column: x => x.categoria_llamado_id,
                        principalTable: "categoria_llamado",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "llamado_gerente_id_fkey",
                        column: x => x.gerente_id,
                        principalTable: "gerente",
                        principalColumn: "id_empleado");
                    table.ForeignKey(
                        name: "llamado_mesa_id_fkey",
                        column: x => x.mesa_id,
                        principalTable: "mesa",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "llamado_mozo_id_fkey",
                        column: x => x.mozo_id,
                        principalTable: "mozo",
                        principalColumn: "id_empleado");
                });

            migrationBuilder.CreateTable(
                name: "mozo_mesa",
                columns: table => new
                {
                    mozo_id = table.Column<int>(type: "integer", nullable: false),
                    mesa_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("mozo_mesa_pkey", x => new { x.mozo_id, x.mesa_id });
                    table.ForeignKey(
                        name: "mozo_mesa_mesa_id_fkey",
                        column: x => x.mesa_id,
                        principalTable: "mesa",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "mozo_mesa_mozo_id_fkey",
                        column: x => x.mozo_id,
                        principalTable: "mozo",
                        principalColumn: "id_empleado");
                });

            migrationBuilder.CreateTable(
                name: "reserva",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    mesa_id = table.Column<int>(type: "integer", nullable: false),
                    cant_comensales = table.Column<int>(type: "integer", nullable: false),
                    nombre_titular = table.Column<string>(type: "text", nullable: false),
                    fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    horario = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    tel_contacto = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("reserva_pkey", x => x.id);
                    table.ForeignKey(
                        name: "reserva_mesa_id_fkey",
                        column: x => x.mesa_id,
                        principalTable: "mesa",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "pago",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cierre_id = table.Column<int>(type: "integer", nullable: true),
                    metodo_pago_id = table.Column<int>(type: "integer", nullable: false),
                    total = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pago_pkey", x => x.id);
                    table.ForeignKey(
                        name: "pago_cierre_id_fkey",
                        column: x => x.cierre_id,
                        principalTable: "cierre",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "pago_metodo_pago_id_fkey",
                        column: x => x.metodo_pago_id,
                        principalTable: "metodo_de_pago",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "ingrediente",
                columns: table => new
                {
                    id_insumo = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ingrediente_pkey", x => x.id_insumo);
                    table.ForeignKey(
                        name: "ingrediente_id_insumo_fkey",
                        column: x => x.id_insumo,
                        principalTable: "insumo",
                        principalColumn: "id_articulo");
                });

            migrationBuilder.CreateTable(
                name: "lote",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    insumo_id = table.Column<int>(type: "integer", nullable: false),
                    bodega_id = table.Column<int>(type: "integer", nullable: false),
                    nombre = table.Column<string>(type: "text", nullable: false),
                    cantidad = table.Column<decimal>(type: "numeric", nullable: false),
                    fecha_adquisicion = table.Column<DateOnly>(type: "date", nullable: false),
                    fecha_vencimiento = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("lote_pkey", x => x.id);
                    table.ForeignKey(
                        name: "lote_bodega_id_fkey",
                        column: x => x.bodega_id,
                        principalTable: "bodega",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "lote_insumo_id_fkey",
                        column: x => x.insumo_id,
                        principalTable: "insumo",
                        principalColumn: "id_articulo");
                });

            migrationBuilder.CreateTable(
                name: "pedido_insumo",
                columns: table => new
                {
                    pedido_id = table.Column<int>(type: "integer", nullable: false),
                    insumo_id = table.Column<int>(type: "integer", nullable: false),
                    precio_compra = table.Column<decimal>(type: "numeric", nullable: false),
                    cantidad = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pedido_insumo_pkey", x => new { x.pedido_id, x.insumo_id });
                    table.ForeignKey(
                        name: "pedido_insumo_insumo_id_fkey",
                        column: x => x.insumo_id,
                        principalTable: "insumo",
                        principalColumn: "id_articulo");
                    table.ForeignKey(
                        name: "pedido_insumo_pedido_id_fkey",
                        column: x => x.pedido_id,
                        principalTable: "pedido",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "restriccion_plato",
                columns: table => new
                {
                    restriccion_id = table.Column<int>(type: "integer", nullable: false),
                    plato_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("restriccion_plato_pkey", x => new { x.restriccion_id, x.plato_id });
                    table.ForeignKey(
                        name: "restriccion_plato_plato_id_fkey",
                        column: x => x.plato_id,
                        principalTable: "plato",
                        principalColumn: "id_articulo");
                    table.ForeignKey(
                        name: "restriccion_plato_restriccion_id_fkey",
                        column: x => x.restriccion_id,
                        principalTable: "restriccion",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "comanda",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    mesa_id = table.Column<int>(type: "integer", nullable: false),
                    pago_id = table.Column<int>(type: "integer", nullable: true),
                    restaurante_id = table.Column<int>(type: "integer", nullable: false),
                    estado_comanda_id = table.Column<int>(type: "integer", nullable: false),
                    cant_comensales = table.Column<int>(type: "integer", nullable: false),
                    hora_inicio = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    hora_fin = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    hora_ultimo_cambio_estado = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("comanda_pkey", x => x.id);
                    table.ForeignKey(
                        name: "comanda_estado_comanda_id_fkey",
                        column: x => x.estado_comanda_id,
                        principalTable: "estado_comanda",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "comanda_mesa_id_fkey",
                        column: x => x.mesa_id,
                        principalTable: "mesa",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "comanda_pago_id_fkey",
                        column: x => x.pago_id,
                        principalTable: "pago",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "comanda_restaurante_id_fkey",
                        column: x => x.restaurante_id,
                        principalTable: "restaurante",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "ingrediente_preparado",
                columns: table => new
                {
                    id_ingrediente = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ingrediente_preparado_pkey", x => x.id_ingrediente);
                    table.ForeignKey(
                        name: "ingrediente_preparado_id_ingrediente_fkey",
                        column: x => x.id_ingrediente,
                        principalTable: "ingrediente",
                        principalColumn: "id_insumo");
                });

            migrationBuilder.CreateTable(
                name: "plato_ingrediente",
                columns: table => new
                {
                    plato_id = table.Column<int>(type: "integer", nullable: false),
                    ingrediente_id = table.Column<int>(type: "integer", nullable: false),
                    opcional = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    cantidad = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("plato_ingrediente_pkey", x => new { x.plato_id, x.ingrediente_id });
                    table.ForeignKey(
                        name: "plato_ingrediente_ingrediente_id_fkey",
                        column: x => x.ingrediente_id,
                        principalTable: "ingrediente",
                        principalColumn: "id_insumo");
                    table.ForeignKey(
                        name: "plato_ingrediente_plato_id_fkey",
                        column: x => x.plato_id,
                        principalTable: "plato",
                        principalColumn: "id_articulo");
                });

            migrationBuilder.CreateTable(
                name: "articulo_comanda",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    comanda_id = table.Column<int>(type: "integer", nullable: false),
                    articulo_id = table.Column<int>(type: "integer", nullable: false),
                    cantidad = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    entregado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    observaciones_ingrediente = table.Column<string>(type: "text", nullable: true),
                    observaciones_generales = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("articulo_comanda_pkey", x => x.id);
                    table.ForeignKey(
                        name: "articulo_comanda_articulo_id_fkey",
                        column: x => x.articulo_id,
                        principalTable: "articulo",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "articulo_comanda_comanda_id_fkey",
                        column: x => x.comanda_id,
                        principalTable: "comanda",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "ingrediente_ingrediente_preparado",
                columns: table => new
                {
                    ingrediente_id = table.Column<int>(type: "integer", nullable: false),
                    ingrediente_preparado_id = table.Column<int>(type: "integer", nullable: false),
                    cantidad = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ingrediente_ingrediente_preparado_pkey", x => new { x.ingrediente_id, x.ingrediente_preparado_id });
                    table.ForeignKey(
                        name: "ingrediente_ingrediente_preparado_ingrediente_id_fkey",
                        column: x => x.ingrediente_id,
                        principalTable: "ingrediente",
                        principalColumn: "id_insumo");
                    table.ForeignKey(
                        name: "ingrediente_ingrediente_preparado_ingrediente_preparado_id_fkey",
                        column: x => x.ingrediente_preparado_id,
                        principalTable: "ingrediente_preparado",
                        principalColumn: "id_ingrediente");
                });

            migrationBuilder.CreateIndex(
                name: "IX_articulo_carta_id",
                table: "articulo",
                column: "carta_id");

            migrationBuilder.CreateIndex(
                name: "IX_articulo_restaurante_id",
                table: "articulo",
                column: "restaurante_id");

            migrationBuilder.CreateIndex(
                name: "IX_articulo_comanda_articulo_id",
                table: "articulo_comanda",
                column: "articulo_id");

            migrationBuilder.CreateIndex(
                name: "IX_articulo_comanda_comanda_id",
                table: "articulo_comanda",
                column: "comanda_id");

            migrationBuilder.CreateIndex(
                name: "IX_articulo_configuracion_articulo_configuracion_articulo_id",
                table: "articulo_configuracion_articulo",
                column: "configuracion_articulo_id");

            migrationBuilder.CreateIndex(
                name: "IX_bodega_restaurante_id",
                table: "bodega",
                column: "restaurante_id");

            migrationBuilder.CreateIndex(
                name: "IX_bodega_tipo_bodega_id",
                table: "bodega",
                column: "tipo_bodega_id");

            migrationBuilder.CreateIndex(
                name: "IX_carta_restaurante_id",
                table: "carta",
                column: "restaurante_id");

            migrationBuilder.CreateIndex(
                name: "categoria_insumo_descripcion_key",
                table: "categoria_insumo",
                column: "descripcion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_categoria_insumo_proveedor_proveedor_id",
                table: "categoria_insumo_proveedor",
                column: "proveedor_id");

            migrationBuilder.CreateIndex(
                name: "categoria_llamado_descripcion_key",
                table: "categoria_llamado",
                column: "descripcion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "categoria_plato_descripcion_key",
                table: "categoria_plato",
                column: "descripcion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cierre_restaurante_id",
                table: "cierre",
                column: "restaurante_id");

            migrationBuilder.CreateIndex(
                name: "IX_cierre_turno_laboral_id",
                table: "cierre",
                column: "turno_laboral_id");

            migrationBuilder.CreateIndex(
                name: "IX_comanda_estado_comanda_id",
                table: "comanda",
                column: "estado_comanda_id");

            migrationBuilder.CreateIndex(
                name: "IX_comanda_mesa_id",
                table: "comanda",
                column: "mesa_id");

            migrationBuilder.CreateIndex(
                name: "IX_comanda_pago_id",
                table: "comanda",
                column: "pago_id");

            migrationBuilder.CreateIndex(
                name: "IX_comanda_restaurante_id",
                table: "comanda",
                column: "restaurante_id");

            migrationBuilder.CreateIndex(
                name: "configuracion_articulo_descripcion_key",
                table: "configuracion_articulo",
                column: "descripcion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_empleado_restaurante_id",
                table: "empleado",
                column: "restaurante_id");

            migrationBuilder.CreateIndex(
                name: "IX_empleado_turno_laboral_turno_laboral_id",
                table: "empleado_turno_laboral",
                column: "turno_laboral_id");

            migrationBuilder.CreateIndex(
                name: "estado_comanda_descripcion_key",
                table: "estado_comanda",
                column: "descripcion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "estado_mesa_descripcion_key",
                table: "estado_mesa",
                column: "descripcion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "estado_pedido_descripcion_key",
                table: "estado_pedido",
                column: "descripcion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_fila_virtual_restaurante_id",
                table: "fila_virtual",
                column: "restaurante_id");

            migrationBuilder.CreateIndex(
                name: "IX_grilla_restaurante_id",
                table: "grilla",
                column: "restaurante_id");

            migrationBuilder.CreateIndex(
                name: "IX_ingrediente_ingrediente_preparado_ingrediente_preparado_id",
                table: "ingrediente_ingrediente_preparado",
                column: "ingrediente_preparado_id");

            migrationBuilder.CreateIndex(
                name: "IX_insumo_categoria_insumo_id",
                table: "insumo",
                column: "categoria_insumo_id");

            migrationBuilder.CreateIndex(
                name: "IX_insumo_unidad_medida_id",
                table: "insumo",
                column: "unidad_medida_id");

            migrationBuilder.CreateIndex(
                name: "IX_llamado_categoria_llamado_id",
                table: "llamado",
                column: "categoria_llamado_id");

            migrationBuilder.CreateIndex(
                name: "IX_llamado_gerente_id",
                table: "llamado",
                column: "gerente_id");

            migrationBuilder.CreateIndex(
                name: "IX_llamado_mesa_id",
                table: "llamado",
                column: "mesa_id");

            migrationBuilder.CreateIndex(
                name: "IX_llamado_mozo_id",
                table: "llamado",
                column: "mozo_id");

            migrationBuilder.CreateIndex(
                name: "IX_lote_bodega_id",
                table: "lote",
                column: "bodega_id");

            migrationBuilder.CreateIndex(
                name: "IX_lote_insumo_id",
                table: "lote",
                column: "insumo_id");

            migrationBuilder.CreateIndex(
                name: "IX_mesa_dimension_mesa_id",
                table: "mesa",
                column: "dimension_mesa_id");

            migrationBuilder.CreateIndex(
                name: "IX_mesa_estado_mesa_id",
                table: "mesa",
                column: "estado_mesa_id");

            migrationBuilder.CreateIndex(
                name: "IX_mesa_grilla_id",
                table: "mesa",
                column: "grilla_id");

            migrationBuilder.CreateIndex(
                name: "metodo_de_pago_descripcion_key",
                table: "metodo_de_pago",
                column: "descripcion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_metodo_de_pago_restaurante_metodo_de_pago_id",
                table: "metodo_de_pago_restaurante",
                column: "metodo_de_pago_id");

            migrationBuilder.CreateIndex(
                name: "IX_mozo_mesa_mesa_id",
                table: "mozo_mesa",
                column: "mesa_id");

            migrationBuilder.CreateIndex(
                name: "IX_notificacion_restaurante_id",
                table: "notificacion",
                column: "restaurante_id");

            migrationBuilder.CreateIndex(
                name: "IX_pago_cierre_id",
                table: "pago",
                column: "cierre_id");

            migrationBuilder.CreateIndex(
                name: "IX_pago_metodo_pago_id",
                table: "pago",
                column: "metodo_pago_id");

            migrationBuilder.CreateIndex(
                name: "IX_pedido_estado_pedido_id",
                table: "pedido",
                column: "estado_pedido_id");

            migrationBuilder.CreateIndex(
                name: "IX_pedido_proveedor_id",
                table: "pedido",
                column: "proveedor_id");

            migrationBuilder.CreateIndex(
                name: "IX_pedido_insumo_insumo_id",
                table: "pedido_insumo",
                column: "insumo_id");

            migrationBuilder.CreateIndex(
                name: "IX_plato_categoria_plato_id",
                table: "plato",
                column: "categoria_plato_id");

            migrationBuilder.CreateIndex(
                name: "IX_plato_tipo_plato_id",
                table: "plato",
                column: "tipo_plato_id");

            migrationBuilder.CreateIndex(
                name: "IX_plato_ingrediente_ingrediente_id",
                table: "plato_ingrediente",
                column: "ingrediente_id");

            migrationBuilder.CreateIndex(
                name: "IX_proveedor_restaurante_id",
                table: "proveedor",
                column: "restaurante_id");

            migrationBuilder.CreateIndex(
                name: "IX_reserva_mesa_id",
                table: "reserva",
                column: "mesa_id");

            migrationBuilder.CreateIndex(
                name: "IX_restaurante_direccion_id",
                table: "restaurante",
                column: "direccion_id");

            migrationBuilder.CreateIndex(
                name: "restriccion_descripcion_key",
                table: "restriccion",
                column: "descripcion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_restriccion_plato_plato_id",
                table: "restriccion_plato",
                column: "plato_id");

            migrationBuilder.CreateIndex(
                name: "IX_sugerencia_plato_ia_restaurante_id",
                table: "sugerencia_plato_ia",
                column: "restaurante_id");

            migrationBuilder.CreateIndex(
                name: "tipo_bodega_descripcion_key",
                table: "tipo_bodega",
                column: "descripcion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "tipo_plato_descripcion_key",
                table: "tipo_plato",
                column: "descripcion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_turno_fila_fila_virtual_id",
                table: "turno_fila",
                column: "fila_virtual_id");

            migrationBuilder.CreateIndex(
                name: "IX_turno_laboral_restaurante_id",
                table: "turno_laboral",
                column: "restaurante_id");

            migrationBuilder.CreateIndex(
                name: "unidad_medida_nombre_key",
                table: "unidad_medida",
                column: "nombre",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "articulo_comanda");

            migrationBuilder.DropTable(
                name: "articulo_configuracion_articulo");

            migrationBuilder.DropTable(
                name: "categoria_insumo_proveedor");

            migrationBuilder.DropTable(
                name: "cocina");

            migrationBuilder.DropTable(
                name: "empleado_turno_laboral");

            migrationBuilder.DropTable(
                name: "ingrediente_ingrediente_preparado");

            migrationBuilder.DropTable(
                name: "llamado");

            migrationBuilder.DropTable(
                name: "lote");

            migrationBuilder.DropTable(
                name: "MesaMozo");

            migrationBuilder.DropTable(
                name: "metodo_de_pago_restaurante");

            migrationBuilder.DropTable(
                name: "mozo_mesa");

            migrationBuilder.DropTable(
                name: "notificacion");

            migrationBuilder.DropTable(
                name: "pedido_insumo");

            migrationBuilder.DropTable(
                name: "plato_ingrediente");

            migrationBuilder.DropTable(
                name: "PlatoRestriccion");

            migrationBuilder.DropTable(
                name: "reserva");

            migrationBuilder.DropTable(
                name: "restriccion_plato");

            migrationBuilder.DropTable(
                name: "sugerencia_plato_ia");

            migrationBuilder.DropTable(
                name: "turno_fila");

            migrationBuilder.DropTable(
                name: "comanda");

            migrationBuilder.DropTable(
                name: "configuracion_articulo");

            migrationBuilder.DropTable(
                name: "ingrediente_preparado");

            migrationBuilder.DropTable(
                name: "categoria_llamado");

            migrationBuilder.DropTable(
                name: "gerente");

            migrationBuilder.DropTable(
                name: "bodega");

            migrationBuilder.DropTable(
                name: "mozo");

            migrationBuilder.DropTable(
                name: "pedido");

            migrationBuilder.DropTable(
                name: "plato");

            migrationBuilder.DropTable(
                name: "restriccion");

            migrationBuilder.DropTable(
                name: "fila_virtual");

            migrationBuilder.DropTable(
                name: "estado_comanda");

            migrationBuilder.DropTable(
                name: "mesa");

            migrationBuilder.DropTable(
                name: "pago");

            migrationBuilder.DropTable(
                name: "ingrediente");

            migrationBuilder.DropTable(
                name: "tipo_bodega");

            migrationBuilder.DropTable(
                name: "empleado");

            migrationBuilder.DropTable(
                name: "estado_pedido");

            migrationBuilder.DropTable(
                name: "proveedor");

            migrationBuilder.DropTable(
                name: "categoria_plato");

            migrationBuilder.DropTable(
                name: "tipo_plato");

            migrationBuilder.DropTable(
                name: "dimension_mesa");

            migrationBuilder.DropTable(
                name: "estado_mesa");

            migrationBuilder.DropTable(
                name: "grilla");

            migrationBuilder.DropTable(
                name: "cierre");

            migrationBuilder.DropTable(
                name: "metodo_de_pago");

            migrationBuilder.DropTable(
                name: "insumo");

            migrationBuilder.DropTable(
                name: "turno_laboral");

            migrationBuilder.DropTable(
                name: "categoria_insumo");

            migrationBuilder.DropTable(
                name: "articulo");

            migrationBuilder.DropTable(
                name: "unidad_medida");

            migrationBuilder.DropTable(
                name: "carta");

            migrationBuilder.DropTable(
                name: "restaurante");

            migrationBuilder.DropTable(
                name: "ubicacion");
        }
    }
}
