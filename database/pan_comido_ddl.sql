-- ============================================================
-- PAN COMIDO - Script de creación de base de datos (PostgreSQL / Supabase)
-- Grupo 5 - "No se deJava"
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

CREATE TABLE estado_pago (
    id              SERIAL PRIMARY KEY,
    descripcion     TEXT NOT NULL UNIQUE
);

-- Familias tipográficas predefinidas con categoría estética
CREATE TABLE familia_tipografica (
    id                  SERIAL PRIMARY KEY,
    categoria           TEXT NOT NULL,       -- 'Moderna', 'Clásica', 'Rústica'
    tipografia_titulo   TEXT NOT NULL,       -- tipografía para títulos/encabezados
    tipografia_cuerpo   TEXT NOT NULL        -- tipografía para texto general
);

-- ============================================================
-- 2. RESTAURANTE Y DEPENDENCIAS DIRECTAS
-- ============================================================

CREATE TABLE restaurante (
    id                      SERIAL PRIMARY KEY,
    direccion_id            INTEGER NOT NULL REFERENCES ubicacion(id),
    familia_tipografica_id  INTEGER REFERENCES familia_tipografica(id),
    nombre                  TEXT NOT NULL,
    imagen                  TEXT,
    color_principal         TEXT,
    color_secundario        TEXT,
    link_resena_google_maps TEXT
);

CREATE TABLE carta (
    id              SERIAL PRIMARY KEY,
    restaurante_id  INTEGER NOT NULL REFERENCES restaurante(id)
);

CREATE TABLE turno_laboral (
    id                      SERIAL PRIMARY KEY,
    restaurante_id          INTEGER NOT NULL REFERENCES restaurante(id),
    horario_laboral_inicio  TIME NOT NULL,
    horario_laboral_fin     TIME NOT NULL,
    es_nocturno             BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE grilla (
    id              SERIAL PRIMARY KEY,
    restaurante_id  INTEGER NOT NULL REFERENCES restaurante(id),
    cant_columnas   INTEGER NOT NULL,
    cant_filas      INTEGER NOT NULL
);

CREATE TABLE fila_virtual (
    id              SERIAL PRIMARY KEY,
    restaurante_id  INTEGER NOT NULL REFERENCES restaurante(id) ON DELETE CASCADE,
    habilitada      BOOLEAN NOT NULL DEFAULT FALSE,
    tiempo_promedio_comida_minutos INTEGER NOT NULL DEFAULT 40
);

CREATE TABLE bodega (
    id              SERIAL PRIMARY KEY,
    restaurante_id  INTEGER NOT NULL REFERENCES restaurante(id),
    tipo_bodega_id  INTEGER NOT NULL REFERENCES tipo_bodega(id),
    nombre          TEXT NOT NULL,
    eliminado       BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE proveedor (
    id                      SERIAL PRIMARY KEY,
    restaurante_id          INTEGER NOT NULL REFERENCES restaurante(id),
    nombre                  TEXT NOT NULL,
    numero_telefono_wsp     TEXT,
    eliminado               BOOLEAN NOT NULL DEFAULT FALSE
);

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

CREATE TABLE porcentaje_categoria_plato (
    restaurante_id      INTEGER NOT NULL REFERENCES restaurante(id),
    categoria_plato_id  INTEGER NOT NULL REFERENCES categoria_plato(id),
    porcentaje          DECIMAL NOT NULL DEFAULT 20,
    PRIMARY KEY (restaurante_id, categoria_plato_id)
);

CREATE TABLE porcentaje_categoria_bebida (
    restaurante_id      INTEGER NOT NULL REFERENCES restaurante(id),
    categoria_insumo_id INTEGER NOT NULL REFERENCES categoria_insumo(id),
    porcentaje          DECIMAL NOT NULL DEFAULT 20,
    PRIMARY KEY (restaurante_id, categoria_insumo_id)
);

-- ============================================================
-- 3. EMPLEADOS Y ROLES
-- ============================================================

CREATE TABLE empleado (
    id                  SERIAL PRIMARY KEY,
    restaurante_id      INTEGER NOT NULL REFERENCES restaurante(id),
    nombre              TEXT NOT NULL,
    email               TEXT NOT NULL,
    contrasena          TEXT NOT NULL,
    estado              TEXT NOT NULL DEFAULT 'activo',
    eliminado           BOOLEAN NOT NULL DEFAULT FALSE,
    reset_token         TEXT,
    reset_token_expires TIMESTAMP
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

CREATE TABLE empleado_turno_laboral (
    empleado_id      INTEGER NOT NULL REFERENCES empleado(id),
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
    activo              BOOLEAN NOT NULL DEFAULT TRUE,
    codigo_invitacion   TEXT,
    cant_personas_max   INTEGER NOT NULL,
    tipo_elemento       INTEGER NOT NULL DEFAULT 1,
    color               TEXT,
    texto_objeto        TEXT
);

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

CREATE TABLE estado_turno_mesa (
    id INT PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL
);

CREATE TABLE turno_fila (
    id                  SERIAL PRIMARY KEY,
    fila_virtual_id     INTEGER NOT NULL REFERENCES fila_virtual(id),
    numero              INTEGER NOT NULL,
    cantidad_comensales INTEGER NOT NULL,
    fecha_hora_ingreso  TIMESTAMP NOT NULL DEFAULT NOW(),
    estado_turno_mesa_id INTEGER NOT NULL REFERENCES estado_turno_mesa(id),
    mesa_asignada_id    INTEGER REFERENCES mesa(id),
    fecha_hora_asignacion TIMESTAMP,
    comanda_pre_armada_id INTEGER
);

-- ============================================================
-- 7. ARTÍCULOS
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
    url_imagen              TEXT,
    eliminado               BOOLEAN NOT NULL DEFAULT FALSE,
    es_precio_manual        BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE articulo_configuracion_articulo (
    articulo_id                 INTEGER NOT NULL REFERENCES articulo(id),
    configuracion_articulo_id   INTEGER NOT NULL REFERENCES configuracion_articulo(id),
    PRIMARY KEY (articulo_id, configuracion_articulo_id)
);

CREATE TABLE plato (
    id_articulo             INTEGER PRIMARY KEY REFERENCES articulo(id),
    tipo_plato_id           INTEGER NOT NULL REFERENCES tipo_plato(id),
    categoria_plato_id      INTEGER NOT NULL REFERENCES categoria_plato(id),
    tiempo_preparacion_base INTEGER NOT NULL,
    destacado               BOOLEAN NOT NULL DEFAULT FALSE,
    sugerencia              BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE TABLE insumo (
    id_articulo         INTEGER PRIMARY KEY REFERENCES articulo(id),
    categoria_insumo_id INTEGER NOT NULL REFERENCES categoria_insumo(id),
    unidad_medida_id    INTEGER NOT NULL REFERENCES unidad_medida(id),
    stock_minimo        DECIMAL NOT NULL DEFAULT 0,
    stock_recomendado   DECIMAL NOT NULL DEFAULT 0
);

CREATE TABLE ingrediente (
    id_insumo   INTEGER PRIMARY KEY REFERENCES insumo(id_articulo)
);

CREATE TABLE ingrediente_preparado (
    id_ingrediente      INTEGER PRIMARY KEY REFERENCES ingrediente(id_insumo),
    rendimiento_base    DECIMAL NOT NULL DEFAULT 1
);

CREATE TABLE ingrediente_ingrediente_preparado (
    ingrediente_id           INTEGER NOT NULL REFERENCES ingrediente(id_insumo),
    ingrediente_preparado_id INTEGER NOT NULL REFERENCES ingrediente_preparado(id_ingrediente),
    cantidad                 DECIMAL NOT NULL,
    PRIMARY KEY (ingrediente_id, ingrediente_preparado_id)
);

CREATE TABLE bebida_preparada (
    id_articulo INTEGER PRIMARY KEY REFERENCES articulo(id)
);

CREATE TABLE bebida_preparada_insumo (
    bebida_preparada_id INTEGER NOT NULL REFERENCES bebida_preparada(id_articulo),
    insumo_id           INTEGER NOT NULL REFERENCES insumo(id_articulo),
    cantidad            NUMERIC NOT NULL,
    PRIMARY KEY (bebida_preparada_id, insumo_id)
);

CREATE TABLE restriccion_plato (
    restriccion_id  INTEGER NOT NULL REFERENCES restriccion(id),
    plato_id        INTEGER NOT NULL REFERENCES plato(id_articulo),
    PRIMARY KEY (restriccion_id, plato_id)
);

CREATE TABLE plato_ingrediente (
    plato_id        INTEGER NOT NULL REFERENCES plato(id_articulo),
    ingrediente_id  INTEGER NOT NULL REFERENCES ingrediente(id_insumo),
    opcional        BOOLEAN NOT NULL DEFAULT FALSE,
    cantidad        DECIMAL NOT NULL,
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
    fecha_vencimiento   DATE,
    eliminado           BOOLEAN NOT NULL DEFAULT FALSE
);

-- ============================================================
-- 9. PAGOS Y CIERRES
-- ============================================================

CREATE TABLE metodo_de_pago_restaurante (
    restaurante_id      INTEGER NOT NULL REFERENCES restaurante(id),
    metodo_de_pago_id   INTEGER NOT NULL REFERENCES metodo_de_pago(id),
    habilitado          BOOLEAN NOT NULL DEFAULT TRUE,
    PRIMARY KEY (restaurante_id, metodo_de_pago_id)
);

-- Datos bancarios que el gerente carga para que el comensal transfiera.
-- 1:1 con restaurante (se pisa con UPDATE, no se guarda historial).
CREATE TABLE datos_transferencia (
    id                  SERIAL PRIMARY KEY,
    restaurante_id      INTEGER NOT NULL UNIQUE REFERENCES restaurante(id),
    alias               TEXT NOT NULL,
    cbu                 TEXT,
    numero_cuenta       TEXT NOT NULL,
    titular_cuenta      TEXT NOT NULL
);

CREATE TABLE cierre (
    id                      SERIAL PRIMARY KEY,
    restaurante_id          INTEGER NOT NULL REFERENCES restaurante(id),
    turno_laboral_id        INTEGER NOT NULL REFERENCES turno_laboral(id),
    diferencia              DECIMAL NOT NULL DEFAULT 0,
    sobrante                DECIMAL NOT NULL DEFAULT 0,
    total_efectivo          DECIMAL NOT NULL DEFAULT 0,
    total_tarjeta           DECIMAL NOT NULL DEFAULT 0,
    total_transferencia     DECIMAL NOT NULL DEFAULT 0,
    total_mercado_pago      DECIMAL NOT NULL DEFAULT 0,
	fecha 					DATE NOT NULL DEFAULT CURRENT_DATE
);

-- NOTA: la tabla `pago` se crea más abajo (sección 10), DESPUÉS de `comanda`,
-- porque ahora `pago` referencia a `comanda` (relación 1 comanda : N pagos).

-- ============================================================
-- 10. COMANDAS Y PAGOS
-- ============================================================

CREATE TABLE comanda (
    id                        SERIAL PRIMARY KEY,
    mesa_id                   INTEGER NOT NULL REFERENCES mesa(id),
    restaurante_id            INTEGER NOT NULL REFERENCES restaurante(id),
    estado_comanda_id         INTEGER NOT NULL REFERENCES estado_comanda(id),
    cant_comensales           INTEGER NOT NULL,
    hora_inicio               TIMESTAMP NOT NULL DEFAULT NOW(),
    hora_fin                  TIMESTAMP,
    hora_ultimo_cambio_estado TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE pago (
    id                  SERIAL PRIMARY KEY,
    comanda_id          INTEGER NOT NULL REFERENCES comanda(id),
    cierre_id           INTEGER REFERENCES cierre(id),
    metodo_pago_id      INTEGER NOT NULL REFERENCES metodo_de_pago(id),
    estado_pago_id      INTEGER NOT NULL REFERENCES estado_pago(id),
    external_reference  TEXT,
    total               DECIMAL NOT NULL,
	fecha_hora 			TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE articulo_comanda (
    id                          SERIAL PRIMARY KEY,
    comanda_id                  INTEGER NOT NULL REFERENCES comanda(id),
    articulo_id                 INTEGER NOT NULL REFERENCES articulo(id),
    cantidad                    INTEGER NOT NULL DEFAULT 1,
    entregado                   BOOLEAN NOT NULL DEFAULT FALSE,
    observaciones_generales     TEXT,
    nombre_comensal             TEXT NOT NULL DEFAULT 'Anónimo' -- agregado para diferenciar quien pidio que
);

CREATE TABLE articulo_comanda_ingrediente_excluido (
    id                  SERIAL PRIMARY KEY,
    articulo_comanda_id INTEGER NOT NULL REFERENCES articulo_comanda(id),
    ingrediente_id      INTEGER NOT NULL REFERENCES ingrediente(id_insumo),
    UNIQUE (articulo_comanda_id, ingrediente_id) -- evita que guarden dos veces "Sin cebolla" en la misma hamburguesa
);

-- ============================================================
-- 11. ENCUESTA
-- ============================================================
CREATE TABLE encuesta_satisfaccion (
    id SERIAL PRIMARY KEY,
    comanda_id INTEGER NOT NULL REFERENCES comanda(id) ON DELETE CASCADE,
    puntuacion_lugar INTEGER NOT NULL CHECK (puntuacion_lugar >= 1 AND puntuacion_lugar <= 5),
    puntuacion_comida INTEGER NOT NULL CHECK (puntuacion_comida >= 1 AND puntuacion_comida <= 5),
    puntuacion_mozo INTEGER NOT NULL CHECK (puntuacion_mozo >= 1 AND puntuacion_mozo <= 5),
    fecha TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- ============================================================
-- 12. PROVEEDORES Y PEDIDOS
-- ============================================================

CREATE TABLE pedido (
    id               SERIAL PRIMARY KEY,
    proveedor_id     INTEGER NOT NULL REFERENCES proveedor(id),
    estado_pedido_id INTEGER NOT NULL REFERENCES estado_pedido(id),
    fecha            DATE NOT NULL DEFAULT CURRENT_DATE
);

CREATE TABLE pedido_insumo (
    pedido_id     INTEGER NOT NULL REFERENCES pedido(id),
    insumo_id     INTEGER NOT NULL REFERENCES insumo(id_articulo),
    precio_compra DECIMAL NOT NULL,
    cantidad      DECIMAL NOT NULL,
    PRIMARY KEY (pedido_id, insumo_id)
);

-- ============================================================
-- 13. CONFIGURACION DE TIEMPOS EXTRA
-- ============================================================
CREATE TABLE regla_tiempo_extra (
    id SERIAL PRIMARY KEY,
    restaurante_id INTEGER NOT NULL REFERENCES restaurante(id) ON DELETE CASCADE,
    porcentaje_ocupacion_hasta INTEGER NOT NULL,
    minutos_extra INTEGER NOT NULL
);

ALTER TABLE turno_fila 
    ADD CONSTRAINT fk_turno_fila_comanda 
    FOREIGN KEY (comanda_pre_armada_id) REFERENCES comanda(id);

COMMIT;
