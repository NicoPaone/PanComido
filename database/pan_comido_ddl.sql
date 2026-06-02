-- ============================================================
-- PAN COMIDO - Script de creación de base de datos (PostgreSQL / Supabase)
-- Grupo 5 - "No se deJava"
-- ============================================================
-- Jerarquía de herencia:
--   Articulo (supertipo)
--   ├── Plato            (id_articulo → articulo.id)
--   └── Insumo           (id_articulo → articulo.id)  [tiene categoria_insumo_id y unidad_medida_id]
--       └── Ingrediente   (id_insumo → insumo.id_articulo)  [tabla marcadora para relaciones]
--           └── IngredientePreparado (id_ingrediente → ingrediente.id_insumo)
--
-- Las bebidas se identifican por categoria_insumo.tipo_aplica = 2 (no tienen tabla propia)
-- ============================================================

BEGIN;

-- ============================================================
-- 1. ENTIDADES FUERTES (sin FK)
-- ============================================================

CREATE TABLE ubicacion (
    id              SERIAL PRIMARY KEY,
    direccion       TEXT NOT NULL,
    ciudad          TEXT NOT NULL,
    codigo_postal   TEXT NOT NULL
);

CREATE TABLE estado_mesa (
    id              SERIAL PRIMARY KEY,
    descripcion     TEXT NOT NULL UNIQUE
);

CREATE TABLE dimension_mesa (
    id              SERIAL PRIMARY KEY,
    imagen          TEXT,
    forma           TEXT NOT NULL
);

CREATE TABLE categoria_llamado (
    id              SERIAL PRIMARY KEY,
    descripcion     TEXT NOT NULL UNIQUE
);

CREATE TABLE estado_comanda (
    id              SERIAL PRIMARY KEY,
    descripcion     TEXT NOT NULL UNIQUE
);

CREATE TABLE configuracion_articulo (
    id              SERIAL PRIMARY KEY,
    descripcion     TEXT NOT NULL UNIQUE
);

CREATE TABLE categoria_plato (
    id              SERIAL PRIMARY KEY,
    descripcion     TEXT NOT NULL UNIQUE
);

CREATE TABLE tipo_plato (
    id              SERIAL PRIMARY KEY,
    descripcion     TEXT NOT NULL UNIQUE
);

CREATE TABLE restriccion (
    id              SERIAL PRIMARY KEY,
    descripcion     TEXT NOT NULL UNIQUE
);

CREATE TABLE categoria_insumo (
    id              SERIAL PRIMARY KEY,
    descripcion     TEXT NOT NULL UNIQUE,
    tipo_aplica     INTEGER NOT NULL  -- 1 = Ingrediente, 2 = Bebida
);

CREATE TABLE unidad_medida (
    id              SERIAL PRIMARY KEY,
    nombre          TEXT NOT NULL UNIQUE
);

CREATE TABLE tipo_bodega (
    id              SERIAL PRIMARY KEY,
    descripcion     TEXT NOT NULL UNIQUE
);

CREATE TABLE estado_pedido (
    id              SERIAL PRIMARY KEY,
    descripcion     TEXT NOT NULL UNIQUE
);

CREATE TABLE metodo_de_pago (
    id              SERIAL PRIMARY KEY,
    descripcion     TEXT NOT NULL UNIQUE
);

-- ============================================================
-- 2. RESTAURANTE Y DEPENDENCIAS DIRECTAS
-- ============================================================

CREATE TABLE restaurante (
    id                  SERIAL PRIMARY KEY,
    direccion_id        INTEGER NOT NULL REFERENCES ubicacion(id),
    nombre              TEXT NOT NULL,
    imagen              TEXT,
    color_principal     TEXT,
    color_secundario    TEXT,
    texto_principal     TEXT,
    texto_secundario    TEXT
);

CREATE TABLE carta (
    id              SERIAL PRIMARY KEY,
    restaurante_id  INTEGER NOT NULL REFERENCES restaurante(id)
);

CREATE TABLE turno_laboral (
    id                      SERIAL PRIMARY KEY,
    restaurante_id          INTEGER NOT NULL REFERENCES restaurante(id),
    horario_laboral_inicio  TIME NOT NULL,
    horario_laboral_fin     TIME NOT NULL
);

CREATE TABLE grilla (
    id              SERIAL PRIMARY KEY,
    restaurante_id  INTEGER NOT NULL REFERENCES restaurante(id),
    cant_columnas   INTEGER NOT NULL,
    cant_filas      INTEGER NOT NULL
);

CREATE TABLE fila_virtual (
    id              SERIAL PRIMARY KEY,
    restaurante_id  INTEGER NOT NULL REFERENCES restaurante(id)
);

CREATE TABLE bodega (
    id              SERIAL PRIMARY KEY,
    restaurante_id  INTEGER NOT NULL REFERENCES restaurante(id),
    tipo_bodega_id  INTEGER NOT NULL REFERENCES tipo_bodega(id),
    nombre          TEXT NOT NULL
);

CREATE TABLE proveedor (
    id                      SERIAL PRIMARY KEY,
    restaurante_id          INTEGER NOT NULL REFERENCES restaurante(id),
    nombre                  TEXT NOT NULL,
    numero_telefono_wsp     TEXT
);

-- Relación N:M CategoriaInsumo <-> Proveedor
CREATE TABLE categoria_insumo_proveedor (
    categoria_insumo_id INTEGER NOT NULL REFERENCES categoria_insumo(id),
    proveedor_id        INTEGER NOT NULL REFERENCES proveedor(id),
    PRIMARY KEY (categoria_insumo_id, proveedor_id)
);

CREATE TABLE notificacion (
    id              SERIAL PRIMARY KEY,
    restaurante_id  INTEGER NOT NULL REFERENCES restaurante(id),
    fecha           TIMESTAMP NOT NULL DEFAULT NOW(),
    descripcion     TEXT NOT NULL,
    resuelta        BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE sugerencia_plato_ia (
    id              SERIAL PRIMARY KEY,
    restaurante_id  INTEGER NOT NULL REFERENCES restaurante(id),
    json            JSONB NOT NULL
);

-- ============================================================
-- 3. EMPLEADOS Y ROLES
-- ============================================================

CREATE TABLE empleado (
    id              SERIAL PRIMARY KEY,
    restaurante_id  INTEGER NOT NULL REFERENCES restaurante(id),
    nombre          TEXT NOT NULL,
    email           TEXT NOT NULL,
    contrasena      TEXT NOT NULL,
    estado          TEXT NOT NULL DEFAULT 'activo'
);

CREATE TABLE gerente (
    id_empleado     INTEGER PRIMARY KEY REFERENCES empleado(id)
);

CREATE TABLE cocina (
    id_empleado     INTEGER PRIMARY KEY REFERENCES empleado(id)
);

CREATE TABLE mozo (
    id_empleado     INTEGER PRIMARY KEY REFERENCES empleado(id),
    activo          BOOLEAN NOT NULL DEFAULT TRUE
);

-- Relación N:M Empleado <-> TurnoLaboral
CREATE TABLE empleado_turno_laboral (
    empleado_id     INTEGER NOT NULL REFERENCES empleado(id),
    turno_laboral_id INTEGER NOT NULL REFERENCES turno_laboral(id),
    PRIMARY KEY (empleado_id, turno_laboral_id)
);

-- ============================================================
-- 4. MESAS Y DEPENDENCIAS
-- ============================================================

CREATE TABLE mesa (
    id                  SERIAL PRIMARY KEY,
    grilla_id           INTEGER NOT NULL REFERENCES grilla(id),
    estado_mesa_id      INTEGER NOT NULL REFERENCES estado_mesa(id),
    dimension_mesa_id   INTEGER NOT NULL REFERENCES dimension_mesa(id),
    posicion_x_inicio   INTEGER NOT NULL,
    posicion_x_fin      INTEGER NOT NULL,
    posicion_y_inicio   INTEGER NOT NULL,
    posicion_y_fin      INTEGER NOT NULL,
    numero              INTEGER NOT NULL,
    codigo_invitacion   TEXT,
    cant_personas_max   INTEGER NOT NULL
);

-- Relación N:M Mozo <-> Mesa
CREATE TABLE mozo_mesa (
    mozo_id     INTEGER NOT NULL REFERENCES mozo(id_empleado),
    mesa_id     INTEGER NOT NULL REFERENCES mesa(id),
    PRIMARY KEY (mozo_id, mesa_id)
);

CREATE TABLE reserva (
    id                  SERIAL PRIMARY KEY,
    mesa_id             INTEGER NOT NULL REFERENCES mesa(id),
    cant_comensales     INTEGER NOT NULL,
    nombre_titular      TEXT NOT NULL,
    fecha               DATE NOT NULL,
    horario             TIME NOT NULL,
    tel_contacto        TEXT
);

-- ============================================================
-- 5. LLAMADOS
-- ============================================================

CREATE TABLE llamado (
    id                      SERIAL PRIMARY KEY,
    mozo_id                 INTEGER REFERENCES mozo(id_empleado),
    gerente_id              INTEGER REFERENCES gerente(id_empleado),
    mesa_id                 INTEGER REFERENCES mesa(id),
    categoria_llamado_id    INTEGER NOT NULL REFERENCES categoria_llamado(id),
    descripcion             TEXT,
    resuelto                BOOLEAN NOT NULL DEFAULT FALSE
);

-- ============================================================
-- 6. FILA VIRTUAL
-- ============================================================

CREATE TABLE turno_fila (
    id                  SERIAL PRIMARY KEY,
    fila_virtual_id     INTEGER NOT NULL REFERENCES fila_virtual(id),
    numero              INTEGER NOT NULL
    -- tiempo_espera es campo calculado en la app
);

-- ============================================================
-- 7. ARTÍCULOS (supertipo) Y TODA LA JERARQUÍA
-- ============================================================

CREATE TABLE articulo (
    id                      SERIAL PRIMARY KEY,
    carta_id                INTEGER REFERENCES carta(id),
    restaurante_id          INTEGER NOT NULL REFERENCES restaurante(id),
    nombre                  TEXT NOT NULL,
    descripcion             TEXT,
    precio_venta_final      DECIMAL,
    precio_ganancia         DECIMAL,
    precio_promocional      DECIMAL,
    url_imagen              TEXT
    -- cantidad_actual es campo calculado en la app
);

-- Relación N:M Articulo <-> ConfiguracionArticulo
CREATE TABLE articulo_configuracion_articulo (
    articulo_id                 INTEGER NOT NULL REFERENCES articulo(id),
    configuracion_articulo_id   INTEGER NOT NULL REFERENCES configuracion_articulo(id),
    PRIMARY KEY (articulo_id, configuracion_articulo_id)
);

-- Subtipo: Plato (hereda de Articulo)
CREATE TABLE plato (
    id_articulo             INTEGER PRIMARY KEY REFERENCES articulo(id),
    tipo_plato_id           INTEGER NOT NULL REFERENCES tipo_plato(id),
    categoria_plato_id      INTEGER NOT NULL REFERENCES categoria_plato(id),
    tiempo_preparacion_base INTEGER NOT NULL, -- en minutos
    destacado               BOOLEAN NOT NULL DEFAULT FALSE,
    sugerencia              BOOLEAN NOT NULL DEFAULT FALSE
    -- costo_base es campo calculado en la app
);

-- Subtipo: Insumo (hereda de Articulo)
CREATE TABLE insumo (
    id_articulo         INTEGER PRIMARY KEY REFERENCES articulo(id),
    categoria_insumo_id INTEGER NOT NULL REFERENCES categoria_insumo(id),
    unidad_medida_id    INTEGER NOT NULL REFERENCES unidad_medida(id),
    stock_minimo        DECIMAL NOT NULL DEFAULT 0
);

-- Subtipo de Insumo: Ingrediente (tabla marcadora para relaciones con Plato e IngredientePreparado)
CREATE TABLE ingrediente (
    id_insumo   INTEGER PRIMARY KEY REFERENCES insumo(id_articulo)
);

-- Sub-subtipo: IngredientePreparado (hereda de Ingrediente)
CREATE TABLE ingrediente_preparado (
    id_ingrediente  INTEGER PRIMARY KEY REFERENCES ingrediente(id_insumo)
);

-- Relación N:M Ingrediente <-> IngredientePreparado (composición)
CREATE TABLE ingrediente_ingrediente_preparado (
    ingrediente_id          INTEGER NOT NULL REFERENCES ingrediente(id_insumo),
    ingrediente_preparado_id INTEGER NOT NULL REFERENCES ingrediente_preparado(id_ingrediente),
    cantidad        DECIMAL NOT NULL, -- por ahora siempre en kg
    PRIMARY KEY (ingrediente_id, ingrediente_preparado_id)
);

-- Relación N:M Restriccion <-> Plato
CREATE TABLE restriccion_plato (
    restriccion_id  INTEGER NOT NULL REFERENCES restriccion(id),
    plato_id        INTEGER NOT NULL REFERENCES plato(id_articulo),
    PRIMARY KEY (restriccion_id, plato_id)
);

-- Relación N:M Plato <-> Ingrediente
CREATE TABLE plato_ingrediente (
    plato_id        INTEGER NOT NULL REFERENCES plato(id_articulo),
    ingrediente_id  INTEGER NOT NULL REFERENCES ingrediente(id_insumo),
    opcional        BOOLEAN NOT NULL DEFAULT FALSE,
    cantidad        DECIMAL NOT NULL, -- por ahora siempre en kg
    PRIMARY KEY (plato_id, ingrediente_id)
);

-- ============================================================
-- 8. LOTES
-- ============================================================

CREATE TABLE lote (
    id                  SERIAL PRIMARY KEY,
    insumo_id           INTEGER NOT NULL REFERENCES insumo(id_articulo),
    bodega_id           INTEGER NOT NULL REFERENCES bodega(id),
    nombre              TEXT NOT NULL,
    cantidad            DECIMAL NOT NULL,
    fecha_adquisicion   DATE NOT NULL,
    fecha_vencimiento   DATE
);

-- ============================================================
-- 9. PAGOS Y CIERRES
-- ============================================================

-- Relación N:M MetodoDePago <-> Restaurante
CREATE TABLE metodo_de_pago_restaurante (
    restaurante_id      INTEGER NOT NULL REFERENCES restaurante(id),
    metodo_de_pago_id   INTEGER NOT NULL REFERENCES metodo_de_pago(id),
    habilitado          BOOLEAN NOT NULL DEFAULT TRUE,
    PRIMARY KEY (restaurante_id, metodo_de_pago_id)
);

CREATE TABLE cierre (
    id                  SERIAL PRIMARY KEY,
    restaurante_id      INTEGER NOT NULL REFERENCES restaurante(id),
    turno_laboral_id    INTEGER NOT NULL REFERENCES turno_laboral(id),
    diferencia          DECIMAL NOT NULL DEFAULT 0,
    sobrante            DECIMAL NOT NULL DEFAULT 0
);

CREATE TABLE pago (
    id                  SERIAL PRIMARY KEY,
    cierre_id           INTEGER NOT NULL REFERENCES cierre(id),
    metodo_pago_id      INTEGER NOT NULL REFERENCES metodo_de_pago(id),
    total               DECIMAL NOT NULL
);

-- ============================================================
-- 10. COMANDAS
-- ============================================================

CREATE TABLE comanda (
    id                  SERIAL PRIMARY KEY,
    mesa_id             INTEGER NOT NULL REFERENCES mesa(id),
    pago_id             INTEGER REFERENCES pago(id),
    restaurante_id      INTEGER NOT NULL REFERENCES restaurante(id),
    estado_comanda_id   INTEGER NOT NULL REFERENCES estado_comanda(id),
    cant_comensales     INTEGER NOT NULL,
    hora_inicio         TIMESTAMP NOT NULL DEFAULT NOW(),
    hora_fin            TIMESTAMP
);

-- Relación N:M Articulo <-> Comanda (con atributos de relación)
CREATE TABLE articulo_comanda (
    id                          SERIAL PRIMARY KEY,
    comanda_id                  INTEGER NOT NULL REFERENCES comanda(id),
    articulo_id                 INTEGER NOT NULL REFERENCES articulo(id),
    cantidad                    INTEGER NOT NULL DEFAULT 1,
    entregado                   BOOLEAN NOT NULL DEFAULT FALSE,
    observaciones_ingrediente   TEXT,
    observaciones_generales     TEXT
);

-- ============================================================
-- 11. PROVEEDORES Y PEDIDOS
-- ============================================================

CREATE TABLE pedido (
    id                  SERIAL PRIMARY KEY,
    proveedor_id        INTEGER NOT NULL REFERENCES proveedor(id),
    estado_pedido_id    INTEGER NOT NULL REFERENCES estado_pedido(id),
    fecha               DATE NOT NULL DEFAULT CURRENT_DATE
);

-- Relación N:M Pedido <-> Insumo
CREATE TABLE pedido_insumo (
    pedido_id       INTEGER NOT NULL REFERENCES pedido(id),
    insumo_id       INTEGER NOT NULL REFERENCES insumo(id_articulo),
    precio_compra   DECIMAL NOT NULL,
    cantidad        DECIMAL NOT NULL,
    PRIMARY KEY (pedido_id, insumo_id)
);

COMMIT;
