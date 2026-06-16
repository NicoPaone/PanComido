-- ============================================================
-- PAN COMIDO - Datos de prueba completos (PostgreSQL / Supabase)
-- Grupo 5 - "No se deJava"
-- Ejecutar DESPUÉS del script DDL y del script SEED
-- ============================================================

BEGIN;

-- ============================================================
-- RESTAURANTE
-- familia_tipografica_id 7 = Rústica (Fredoka One + Source Sans Pro)
-- ============================================================

INSERT INTO ubicacion (id, direccion, ciudad, codigo_postal) VALUES
    (1, 'Av. Corrientes 1234', 'CABA', '1043');

INSERT INTO restaurante (id, direccion_id, familia_tipografica_id, nombre, imagen, color_principal, color_secundario) VALUES
    (1, 1, 7, 'Pan Comido', '/img/logo-pan-comido.png', '#FBAC28', '#C5172E');

INSERT INTO carta (id, restaurante_id) VALUES
    (1, 1);

-- ============================================================
-- TURNOS LABORALES
-- ============================================================

INSERT INTO turno_laboral (id, restaurante_id, horario_laboral_inicio, horario_laboral_fin, es_nocturno) VALUES
    (1, 1, '08:00', '16:00', FALSE),
    (2, 1, '16:00', '00:00', TRUE);

-- ============================================================
-- EMPLEADOS Y ROLES
-- ============================================================

INSERT INTO empleado (id, restaurante_id, nombre, email, contrasena, estado, eliminado) VALUES
    (1, 1, 'Carlos López',    'carlos@pancomido.com',  '$2a$11$v3Wa2R8iZwjWc7RI/ANkIeDbrrQUXOEGdGDFf.dQVaqUNjGm8Tv42',  'activo',   FALSE),
    (2, 1, 'Cocina',          'cocina@pancomido.com',  '$2a$11$v3Wa2R8iZwjWc7RI/ANkIeDbrrQUXOEGdGDFf.dQVaqUNjGm8Tv42',  'activo',   FALSE),
    (3, 1, 'Ana Rodríguez',   'ana@pancomido.com',     '$2a$11$v3Wa2R8iZwjWc7RI/ANkIeDbrrQUXOEGdGDFf.dQVaqUNjGm8Tv42',    'activo',   FALSE),
    (4, 1, 'Pedro Martínez',  'pedro@pancomido.com',   '$2a$11$v3Wa2R8iZwjWc7RI/ANkIeDbrrQUXOEGdGDFf.dQVaqUNjGm8Tv42',    'activo',   FALSE),
    (5, 1, 'Laura Fernández', 'laura@pancomido.com',   '$2a$11$v3Wa2R8iZwjWc7RI/ANkIeDbrrQUXOEGdGDFf.dQVaqUNjGm8Tv42',    'activo',   FALSE),
    (6, 1, 'Diego Sánchez',   'diego@pancomido.com',   '$2a$11$v3Wa2R8iZwjWc7RI/ANkIeDbrrQUXOEGdGDFf.dQVaqUNjGm8Tv42', 'inactivo', TRUE);

SELECT setval('empleado_id_seq', (SELECT MAX(id) FROM empleado));

INSERT INTO empleado (restaurante_id, nombre, email, contrasena, estado)
VALUES (1, 'Lucia Gerente', 'lucia@pancomido.com', '$2a$11$v3Wa2R8iZwjWc7RI/ANkIeDbrrQUXOEGdGDFf.dQVaqUNjGm8Tv42', 'activo');
INSERT INTO gerente (id_empleado) VALUES (currval('empleado_id_seq'));

INSERT INTO empleado (restaurante_id, nombre, email, contrasena, estado)
VALUES (1, 'Martin Mozo', 'martin@pancomido.com', '$2a$11$v3Wa2R8iZwjWc7RI/ANkIeDbrrQUXOEGdGDFf.dQVaqUNjGm8Tv42', 'activo');
INSERT INTO mozo (id_empleado, activo) VALUES (currval('empleado_id_seq'), true);

INSERT INTO empleado (restaurante_id, nombre, email, contrasena, estado)
VALUES (1, 'Sofia Cocina', 'sofia@pancomido.com', '$2a$11$v3Wa2R8iZwjWc7RI/ANkIeDbrrQUXOEGdGDFf.dQVaqUNjGm8Tv42', 'activo');
INSERT INTO cocina (id_empleado) VALUES (currval('empleado_id_seq'));

INSERT INTO gerente (id_empleado) VALUES (1);
INSERT INTO cocina (id_empleado) VALUES (2);

INSERT INTO mozo (id_empleado, activo) VALUES
    (3, TRUE),
    (4, TRUE),
    (5, TRUE),
    (6, FALSE);

INSERT INTO empleado_turno_laboral (empleado_id, turno_laboral_id) VALUES
    (1, 1), (1, 2),
    (2, 1), (2, 2),
    (3, 1),
    (4, 2),
    (5, 1), (5, 2);

-- ============================================================
-- GRILLA Y MESAS
-- ============================================================

INSERT INTO dimension_mesa (id, imagen, forma) VALUES
    (1, '/img/mesa-cuadrada-2.png',    'cuadrada'),
    (2, '/img/mesa-cuadrada-4.png',    'cuadrada'),
    (3, '/img/mesa-rectangular-6.png', 'rectangular'),
    (4, '/img/mesa-rectangular-8.png', 'rectangular'),
    (5, '/img/mesa-redonda-4.png',     'redonda'),
    (6, NULL,                          'horizontal_larga'),
    (7, NULL,                          'horizontal_alta');

INSERT INTO grilla (id, restaurante_id, cant_columnas, cant_filas) VALUES
    (1, 1, 8, 6);

INSERT INTO mesa (id, grilla_id, estado_mesa_id, dimension_mesa_id, posicion_x_inicio, posicion_x_fin, posicion_y_inicio, posicion_y_fin, numero, codigo_invitacion, cant_personas_max) VALUES
    (1,  1, 1, 1, 30,   120,  30,  120,  1,  NULL, 4),
    (2,  1, 3, 1, 30,   120,  165, 255,  2,  NULL, 4),
    (3,  1, 2, 1, 30,   120,  300, 390,  3,  NULL, 4),
    (4,  1, 2, 1, 30,   120,  435, 525,  4,  NULL, 4),
    (5,  1, 3, 1, 30,   120,  570, 660,  5,  NULL, 4),
    (6,  1, 3, 1, 165,  255,  30,  120,  6,  NULL, 4),
    (7,  1, 1, 1, 165,  255,  165, 255,  7,  NULL, 4),
    (8,  1, 1, 1, 165,  255,  300, 390,  8,  NULL, 4),
    (9,  1, 1, 1, 165,  255,  435, 525,  9,  NULL, 4),
    (10, 1, 1, 1, 165,  255,  570, 660,  10, NULL, 4),
    (11, 1, 1, 6, 300,  450,  30,  105,  11, NULL, 4),
    (12, 1, 1, 6, 510,  660,  30,  105,  12, NULL, 4),
    (13, 1, 1, 6, 720,  870,  30,  105,  13, NULL, 4),
    (14, 1, 1, 6, 300,  450,  585, 660,  14, NULL, 4),
    (15, 1, 1, 6, 510,  660,  585, 660,  15, NULL, 4),
    (16, 1, 1, 6, 300,  450,  480, 555,  16, NULL, 4),
    (17, 1, 1, 7, 930,  1005, 30,  180,  17, NULL, 4),
    (18, 1, 1, 6, 510,  660,  480, 555,  18, NULL, 4),
    (19, 1, 1, 7, 1080, 1155, 30,  180,  19, NULL, 4),
    (20, 1, 1, 7, 930,  1005, 240, 390,  20, NULL, 4),
    (21, 1, 1, 7, 1080, 1155, 240, 390,  21, NULL, 4),
    (22, 1, 1, 7, 930,  1005, 450, 600,  22, NULL, 4),
    (23, 1, 1, 7, 1080, 1155, 450, 600,  23, NULL, 4),
    (24, 1, 1, 5, 315,  405,  165, 255,  24, NULL, 4),
    (25, 1, 1, 5, 315,  405,  300, 390,  25, NULL, 4),
    (26, 1, 1, 5, 465,  555,  165, 255,  26, NULL, 4),
    (27, 1, 1, 5, 465,  555,  300, 390,  27, NULL, 4),
    (28, 1, 1, 5, 615,  705,  165, 255,  28, NULL, 4),
    (29, 1, 1, 5, 615,  705,  300, 390,  29, NULL, 4),
    (30, 1, 1, 5, 780,  870,  165, 255,  30, NULL, 4),
    (31, 1, 1, 5, 780,  870,  300, 390,  31, NULL, 4);

INSERT INTO mozo_mesa (mozo_id, mesa_id) VALUES
    (3, 1), (3, 2), (3, 3),
    (4, 4), (4, 5), (4, 6),
    (5, 8);

-- ============================================================
-- RESERVAS
-- ============================================================

INSERT INTO reserva (id, mesa_id, cant_comensales, nombre_titular, fecha, horario, tel_contacto) VALUES
    (1, 5, 3, 'Roberto Gómez',     CURRENT_DATE,     '20:30', '1155667788'),
    (2, 2, 2, 'Lucía Herrera',     CURRENT_DATE + 1, '21:00', '1144556677'),
    (3, 6, 7, 'Familia Rodríguez', CURRENT_DATE + 2, '13:00', '1133445566');

-- ============================================================
-- ARTÍCULOS
-- ============================================================

INSERT INTO articulo (id, carta_id, restaurante_id, nombre, descripcion, precio_venta_final, precio_ganancia, precio_promocional, eliminado) VALUES
    (1,  1, 1, 'Pizza Muzzarella',       'Pizza clásica con mozzarella y salsa de tomate',        4500, 2800, NULL, FALSE),
    (2,  1, 1, 'Pizza Napolitana',        'Pizza con tomate, mozzarella y ajo',                   5000, 3200, NULL, FALSE),
    (3,  1, 1, 'Milanesa Napolitana',     'Milanesa de pollo con jamón, queso y salsa',           6500, 4000, NULL, FALSE),
    (4,  1, 1, 'Hamburguesa Clásica',     'Carne vacuna, lechuga, tomate, cebolla',               5500, 3500, NULL, FALSE),
    (5,  1, 1, 'Ensalada César',          'Lechuga, pollo grillado, croutons, parmesano',         4000, 2500, 3500, FALSE),
    (6,  1, 1, 'Bife de Chorizo',         'Bife de chorizo a la parrilla con guarnición',         8500, 5500, NULL, FALSE),
    (7,  1, 1, 'Spaghetti Bolognesa',     'Pasta con salsa bolognesa casera',                     5000, 3000, NULL, FALSE),
    (8,  1, 1, 'Empanadas de Carne (x3)', 'Empanadas de carne cortada a cuchillo',                3000, 1800, NULL, FALSE),
    (9,  1, 1, 'Papas Fritas',            'Porción de papas fritas',                              2500, 1500, NULL, FALSE),
    (10, 1, 1, 'Ensalada Mixta',          'Lechuga, tomate, cebolla, huevo',                      2000, 1200, NULL, FALSE),
    (11, 1, 1, 'Wok de Pollo y Verduras', 'Pollo salteado con pimiento, cebolla y salsa de soja', 5500, 3300, 4800, FALSE),
    (12, 1, 1, 'Coca-Cola 500ml',         'Gaseosa línea Coca-Cola',                              1500, 900,  NULL, FALSE),
    (13, 1, 1, 'Agua Mineral 500ml',      'Agua mineral sin gas',                                 1000, 600,  NULL, FALSE),
    (14, 1, 1, 'Cerveza Artesanal IPA',   'Pinta de cerveza artesanal IPA',                       2500, 1500, NULL, FALSE),
    (15, 1, 1, 'Vino Malbec (copa)',      'Copa de Malbec Reserva',                               2000, 1200, NULL, FALSE),
    (16, 1, 1, 'Sprite 500ml',            'Gaseosa Sprite',                                       1500, 900,  NULL, FALSE),
    (17, 1, 1, 'Fernet con Coca',         'Fernet Branca con Coca-Cola',                          2500, 1500, NULL, FALSE),
    (18, NULL, 1, 'Harina 000',           NULL, NULL, NULL, NULL, FALSE),
    (19, NULL, 1, 'Mozzarella',           NULL, NULL, NULL, NULL, FALSE),
    (20, NULL, 1, 'Tomate perita',        NULL, NULL, NULL, NULL, FALSE),
    (21, NULL, 1, 'Pechuga de pollo',     NULL, NULL, NULL, NULL, FALSE),
    (22, NULL, 1, 'Aceite de oliva',      NULL, NULL, NULL, NULL, FALSE),
    (23, NULL, 1, 'Crema de leche',       NULL, NULL, NULL, NULL, FALSE),
    (24, NULL, 1, 'Sal',                  NULL, NULL, NULL, NULL, FALSE),
    (25, NULL, 1, 'Pimienta',             NULL, NULL, NULL, NULL, FALSE),
    (26, NULL, 1, 'Lechuga',              NULL, NULL, NULL, NULL, FALSE),
    (27, NULL, 1, 'Huevos',               NULL, NULL, NULL, NULL, FALSE),
    (28, NULL, 1, 'Carne vacuna (bife)',   NULL, NULL, NULL, NULL, FALSE),
    (29, NULL, 1, 'Papa',                 NULL, NULL, NULL, NULL, FALSE),
    (30, NULL, 1, 'Cebolla',              NULL, NULL, NULL, NULL, FALSE),
    (31, NULL, 1, 'Ajo',                  NULL, NULL, NULL, NULL, FALSE),
    (32, NULL, 1, 'Pan de hamburguesa',   NULL, NULL, NULL, NULL, FALSE),
    (33, NULL, 1, 'Fideos secos',         NULL, NULL, NULL, NULL, FALSE),
    (34, NULL, 1, 'Albahaca',             NULL, NULL, NULL, NULL, FALSE),
    (35, NULL, 1, 'Pimiento rojo',        NULL, NULL, NULL, NULL, FALSE),
    (36, NULL, 1, 'Orégano',              NULL, NULL, NULL, NULL, FALSE),
    (37, NULL, 1, 'Jamón cocido',         NULL, NULL, NULL, NULL, FALSE),
    (38, NULL, 1, 'Salsa de tomate casera', NULL, NULL, NULL, NULL, FALSE),
    (39, NULL, 1, 'Masa de pizza',          NULL, NULL, NULL, NULL, FALSE),
    (40, NULL, 1, 'Masa de empanada',       NULL, NULL, NULL, NULL, FALSE),
    (41, NULL, 1, 'Salsa bechamel',         NULL, NULL, NULL, NULL, FALSE);

INSERT INTO articulo_configuracion_articulo (articulo_id, configuracion_articulo_id) VALUES
    (1, 1), (1, 2), (2, 1), (2, 2), (3, 1), (3, 2), (4, 1), (4, 2),
    (5, 1), (5, 2), (6, 1), (6, 2), (7, 1),          (8, 1), (8, 2),
    (9, 1), (9, 2), (10, 1), (10, 2), (11, 1), (11, 2),
    (12, 1), (12, 2), (13, 1), (13, 2), (14, 1), (14, 2),
    (15, 1), (15, 2), (16, 1), (16, 2), (17, 1), (17, 2);

-- ============================================================
-- PLATOS
-- ============================================================

INSERT INTO plato (id_articulo, tipo_plato_id, categoria_plato_id, tiempo_preparacion_base, destacado, sugerencia) VALUES
    (1,  3, 2, 20, TRUE,  TRUE),
    (2,  3, 2, 25, FALSE, FALSE),
    (3,  2, 2, 25, TRUE,  TRUE),
    (4,  5, 2, 15, FALSE, FALSE),
    (5,  6, 1, 10, FALSE, TRUE),
    (6,  4, 2, 30, TRUE,  TRUE),
    (7,  1, 2, 15, FALSE, FALSE),
    (8,  9, 1, 20, FALSE, TRUE),
    (9,  2, 4, 10, FALSE, FALSE),
    (10, 6, 1, 5,  FALSE, FALSE),
    (11, 8, 2, 15, FALSE, TRUE);

-- ============================================================
-- INSUMOS
-- ============================================================

INSERT INTO insumo (id_articulo, categoria_insumo_id, unidad_medida_id, stock_minimo) VALUES
    (12, 13, 5, 5), (13, 13, 5, 3), (14, 12, 5, 2), (15, 12, 5, 2),
    (16, 13, 5, 3), (17, 12, 5, 1),
    (18, 11, 1, 2), (19, 4,  1, 1), (20, 2,  1, 0.5), (21, 3,  1, 1),
    (22, 9,  3, 0.5), (23, 4,  3, 0.3), (24, 8,  1, 2), (25, 8,  2, 1),
    (26, 2,  1, 0.5), (27, 7,  5, 0.3), (28, 3,  1, 1), (29, 2,  1, 0.5),
    (30, 2,  1, 0.3), (31, 8,  5, 1), (32, 11, 5, 0.5), (33, 5,  1, 1),
    (34, 2,  2, 1), (35, 2,  1, 0.3), (36, 8,  2, 1), (37, 3,  1, 0.3),
    (38, 2,  3, 0.5), (39, 11, 6, 0.3), (40, 11, 6, 0.2), (41, 4,  3, 0.3);

-- ============================================================
-- INGREDIENTES
-- ============================================================

INSERT INTO ingrediente (id_insumo) VALUES
    (18), (19), (20), (21), (22), (23), (24), (25),
    (26), (27), (28), (29), (30), (31), (32), (33),
    (34), (35), (36), (37), (38), (39), (40), (41);

INSERT INTO ingrediente_preparado (id_ingrediente) VALUES
    (38), (39), (40), (41);

INSERT INTO ingrediente_ingrediente_preparado (ingrediente_id, ingrediente_preparado_id, cantidad) VALUES
    (20, 38, 1.00), (30, 38, 0.20), (31, 38, 0.05), (22, 38, 0.05), (24, 38, 0.02), (34, 38, 0.02),
    (18, 39, 1.00), (24, 39, 0.02), (22, 39, 0.05),
    (18, 40, 1.00), (24, 40, 0.02), (22, 40, 0.10),
    (18, 41, 0.10), (23, 41, 1.00), (24, 41, 0.01), (25, 41, 0.01);

INSERT INTO restriccion_plato (restriccion_id, plato_id) VALUES
    (2, 1), (2, 9), (3, 9), (1, 10), (2, 10), (3, 6);

INSERT INTO plato_ingrediente (plato_id, ingrediente_id, opcional, cantidad) VALUES
    (1, 39, FALSE, 0.35), (1, 38, FALSE, 0.15), (1, 19, FALSE, 0.25), (1, 36, TRUE,  0.02),
    (2, 39, FALSE, 0.35), (2, 38, FALSE, 0.15), (2, 19, FALSE, 0.25), (2, 31, FALSE, 0.01), (2, 20, FALSE, 0.20),
    (3, 21, FALSE, 0.25), (3, 27, FALSE, 0.05), (3, 18, FALSE, 0.10), (3, 37, FALSE, 0.05), (3, 19, FALSE, 0.10), (3, 38, FALSE, 0.10),
    (4, 28, FALSE, 0.20), (4, 32, FALSE, 0.10), (4, 26, FALSE, 0.05), (4, 20, FALSE, 0.05), (4, 30, TRUE,  0.05),
    (5, 26, FALSE, 0.20), (5, 21, FALSE, 0.15), (5, 27, FALSE, 0.05), (5, 22, FALSE, 0.03), (5, 24, FALSE, 0.01), (5, 25, TRUE, 0.02),
    (6, 28, FALSE, 0.40), (6, 24, FALSE, 0.01), (6, 25, FALSE, 0.01), (6, 22, TRUE,  0.02),
    (7, 33, FALSE, 0.20), (7, 38, FALSE, 0.15), (7, 28, FALSE, 0.15), (7, 30, FALSE, 0.05), (7, 31, FALSE, 0.01),
    (8, 40, FALSE, 0.10), (8, 28, FALSE, 0.15), (8, 30, FALSE, 0.08), (8, 27, FALSE, 0.05), (8, 35, TRUE,  0.03),
    (9, 29, FALSE, 0.30), (9, 22, FALSE, 0.05), (9, 24, FALSE, 0.01),
    (10, 26, FALSE, 0.15), (10, 20, FALSE, 0.10), (10, 30, TRUE, 0.05), (10, 27, FALSE, 0.05), (10, 22, FALSE, 0.02), (10, 24, FALSE, 0.01),
    (11, 21, FALSE, 0.20), (11, 35, FALSE, 0.05), (11, 30, FALSE, 0.05), (11, 22, FALSE, 0.02), (11, 24, FALSE, 0.01), (11, 25, TRUE, 0.01);

-- ============================================================
-- BODEGAS
-- ============================================================

INSERT INTO bodega (id, restaurante_id, tipo_bodega_id, nombre, eliminado) VALUES
    (1, 1, 1, 'Almacén principal', FALSE),
    (2, 1, 2, 'Heladera cocina',   FALSE),
    (3, 1, 3, 'Freezer',           FALSE);

-- ============================================================
-- LOTES (Refactorizado con fechas dinámicas)
-- ============================================================

INSERT INTO lote (id, insumo_id, bodega_id, nombre, cantidad, fecha_adquisicion, fecha_vencimiento) VALUES
    -- Lotes vigentes a largo plazo (vencen en varios meses)
    (1,  18, 1, 'Harina 000',          10,   CURRENT_DATE - 30, CURRENT_DATE + 90),
    (2,  22, 1, 'Aceite oliva',        5,    CURRENT_DATE - 30, CURRENT_DATE + 120),
    (3,  24, 1, 'Sal fina',            3,    CURRENT_DATE - 60, CURRENT_DATE + 180),
    (4,  25, 1, 'Pimienta negra',      0.5,  CURRENT_DATE - 60, CURRENT_DATE + 180),
    (5,  32, 1, 'Pan burger',          20,   CURRENT_DATE - 5,  CURRENT_DATE + 15),

    -- Lote VENCIDO INTENCIONALMENTE (para probar validaciones y mermas)
    (6,  33, 1, 'Fideos spaghetti (Vencido)', 3, CURRENT_DATE - 60, CURRENT_DATE - 5),

    (7,  36, 1, 'Orégano seco',        0.3,  CURRENT_DATE - 30, CURRENT_DATE + 90),
    (8,  12, 1, 'Coca-Cola 500ml',     24,   CURRENT_DATE - 10, NULL),
    (9,  13, 1, 'Agua mineral 500ml',  20,   CURRENT_DATE - 10, NULL),
    (10, 14, 1, 'Cerveza IPA',         12,   CURRENT_DATE - 15, CURRENT_DATE + 180),
    (11, 15, 1, 'Malbec Reserva',      6,    CURRENT_DATE - 30, CURRENT_DATE + 300),
    (12, 16, 1, 'Sprite 500ml',        18,   CURRENT_DATE - 10, NULL),
    (13, 17, 1, 'Fernet 750ml',        4,    CURRENT_DATE - 20, CURRENT_DATE + 365),
    (14, 19, 2, 'Mozzarella',          3,    CURRENT_DATE - 15, CURRENT_DATE + 20),
    (15, 19, 2, 'Mozzarella',          2,    CURRENT_DATE - 5,  CURRENT_DATE + 30),
    (16, 20, 2, 'Tomate perita',       4,    CURRENT_DATE - 5,  CURRENT_DATE + 10),
    (17, 21, 2, 'Pechuga (fresca)',    1.5,  CURRENT_DATE - 2,  CURRENT_DATE + 5),
    (18, 23, 2, 'Crema',               2,    CURRENT_DATE - 5,  CURRENT_DATE + 15),
    (19, 26, 2, 'Lechuga',             1,    CURRENT_DATE - 2,  CURRENT_DATE + 4),

    -- Lote VENCE HOY (para alertas de uso rápido)
    (20, 27, 2, 'Huevos (Vence Hoy)',  12,   CURRENT_DATE - 10, CURRENT_DATE),

    (21, 29, 2, 'Papa',                8,    CURRENT_DATE - 10, CURRENT_DATE + 30),
    (22, 30, 2, 'Cebolla',             4,    CURRENT_DATE - 5,  CURRENT_DATE + 15),
    (23, 31, 2, 'Ajo',                 1,    CURRENT_DATE - 30, CURRENT_DATE + 60),
    (24, 34, 2, 'Albahaca fresca',     0.2,  CURRENT_DATE - 1,  CURRENT_DATE + 5),
    (25, 35, 2, 'Pimiento rojo',       2,    CURRENT_DATE - 5,  CURRENT_DATE + 10),
    (26, 37, 2, 'Jamón cocido',        2,    CURRENT_DATE - 3,  CURRENT_DATE + 10),

    -- INGREDIENTES PREPARADOS (Siempre vigentes para las pruebas)
    (27, 38, 2, 'Salsa tomate prep',   3,    CURRENT_DATE - 1,  CURRENT_DATE + 5),
    (28, 39, 2, 'Masa pizza prep',     8,    CURRENT_DATE - 1,  CURRENT_DATE + 3),
    (29, 40, 2, 'Masa empanada prep',  15,   CURRENT_DATE - 1,  CURRENT_DATE + 4),
    (30, 41, 2, 'Bechamel prep',       2,    CURRENT_DATE - 1,  CURRENT_DATE + 4),

    (31, 21, 3, 'Pechuga (congelada)', 1.5,  CURRENT_DATE - 10, CURRENT_DATE + 90),
    (32, 28, 3, 'Bife angosto',        5,    CURRENT_DATE - 5,  CURRENT_DATE + 15);


-- ============================================================
-- MÉTODOS DE PAGO HABILITADOS
-- ============================================================

INSERT INTO metodo_de_pago_restaurante (restaurante_id, metodo_de_pago_id, habilitado) VALUES
    (1, 1, TRUE), (1, 2, TRUE), (1, 3, TRUE), (1, 4, TRUE);

-- ============================================================
-- PORCENTAJES DE GANANCIA
-- categoria_plato: 1=Entrada, 2=Principal, 3=Postre, 4=Guarnición
-- categoria_insumo: 12=Con alcohol, 13=Sin alcohol
-- ============================================================

INSERT INTO porcentaje_categoria_plato (restaurante_id, categoria_plato_id, porcentaje) VALUES
    (1, 1, 20),  -- Entrada
    (1, 2, 20),  -- Principal
    (1, 3, 20),  -- Postre
    (1, 4, 20);  -- Guarnición

INSERT INTO porcentaje_categoria_bebida (restaurante_id, categoria_insumo_id, porcentaje) VALUES
    (1, 12, 20),  -- Con alcohol
    (1, 13, 20);  -- Sin alcohol

-- ============================================================
-- CIERRES Y PAGOS
-- ============================================================

INSERT INTO cierre (id, restaurante_id, turno_laboral_id, diferencia, sobrante, total_efectivo, total_tarjeta, total_transferencia, total_mercado_pago) VALUES
    (1, 1, 1, -150.00, 0, 28500.00, 15200.00, 0.00, 8300.00);

-- ============================================================
-- COMANDAS
-- ============================================================

INSERT INTO comanda (id, mesa_id, restaurante_id, estado_comanda_id, cant_comensales, hora_inicio, hora_fin) VALUES
    (1, 3, 1, 2, 3, NOW() - INTERVAL '25 minutes', NULL),
    (2, 4, 1, 1, 5, NOW() - INTERVAL '5 minutes',  NULL),
    (3, 1, 1, 3, 2, NOW() - INTERVAL '3 hours',     NOW() - INTERVAL '2 hours');

-- ============================================================
-- PAGOS (deben ir DESPUÉS de comanda: pago.comanda_id la referencia)
-- ============================================================

INSERT INTO pago (id, comanda_id, cierre_id, metodo_pago_id, estado_pago_id, external_reference, total) VALUES
    (1, 3, 1, 1, 2, NULL, 28500.00);

INSERT INTO articulo_comanda (comanda_id, articulo_id, cantidad, entregado, observaciones_ingrediente, observaciones_generales) VALUES
    (1, 1,  1, FALSE, 'Sin orégano', NULL),
    (1, 12, 2, TRUE,  NULL,          NULL),
    (1, 6,  1, FALSE, NULL,          'Cocción a punto'),
    (1, 9,  1, TRUE,  NULL,          NULL),
    (2, 8,  2, FALSE, NULL,          NULL),
    (2, 3,  1, FALSE, 'Sin jamón',   'Que no toque la ensalada'),
    (2, 4,  2, FALSE, 'Sin cebolla', NULL),
    (2, 5,  1, FALSE, NULL,          'Aderezo aparte'),
    (2, 14, 2, FALSE, NULL,          NULL),
    (2, 15, 1, FALSE, NULL,          NULL),
    (2, 13, 2, FALSE, NULL,          NULL),
    (3, 2,  1, TRUE,  NULL,          NULL),
    (3, 11, 1, TRUE,  'Sin pimienta', NULL),
    (3, 16, 2, TRUE,  NULL,          NULL);

-- ============================================================
-- LLAMADOS
-- ============================================================

INSERT INTO llamado (id, mozo_id, gerente_id, categoria_llamado_id, descripcion, resuelto) VALUES
    (1, 3, NULL, 1, 'Mesa 3 pidió hielo para las bebidas',  FALSE),
    (2, 3, NULL, 6, 'Mesa 3 pidió más pan',                 TRUE),
    (3, 4, 1,   3, 'Mesa 4 quiere hablar con el encargado', FALSE);

-- ============================================================
-- FILA VIRTUAL
-- ============================================================

INSERT INTO fila_virtual (id, restaurante_id, habilitada) VALUES (1, 1, TRUE);

INSERT INTO turno_fila (id, fila_virtual_id, numero) VALUES
    (1, 1, 1), (2, 1, 2), (3, 1, 3);

-- ============================================================
-- PROVEEDORES
-- ============================================================

INSERT INTO proveedor (id, restaurante_id, nombre, numero_telefono_wsp, eliminado) VALUES
    (1, 1, 'Verdulería Don José',  '1122334455', FALSE),
    (2, 1, 'Carnicería El Gaucho', '1133445566', FALSE),
    (3, 1, 'Distribuidora Central','1144556677', FALSE),
    (4, 1, 'Lácteos del Campo',    '1155667788', FALSE),
    (5, 1, 'Panadería San Martín', '1166778899', FALSE);

INSERT INTO categoria_insumo_proveedor (categoria_insumo_id, proveedor_id) VALUES
    (1, 1), (2, 1), (3, 2), (5, 3), (9, 3),
    (12, 3), (13, 3), (4, 4), (7, 4), (11, 5);

-- ============================================================
-- PEDIDOS
-- ============================================================

INSERT INTO pedido (id, proveedor_id, estado_pedido_id, fecha) VALUES
    (1, 1, 1, CURRENT_DATE),
    (2, 2, 2, CURRENT_DATE - 1),
    (3, 3, 3, CURRENT_DATE - 3),
    (4, 1, 3, CURRENT_DATE - 14),
    (5, 2, 3, CURRENT_DATE - 10),
    (6, 3, 3, CURRENT_DATE - 12),
    (7, 4, 3, CURRENT_DATE - 7),
    (8, 5, 3, CURRENT_DATE - 9);

INSERT INTO pedido_insumo (pedido_id, insumo_id, precio_compra, cantidad) VALUES
    (1, 20, 800,  5), (1, 26, 600,  2), (1, 30, 500,  3),
    (2, 21, 2500, 5), (2, 28, 4500, 3),
    (3, 12, 500,  24), (3, 13, 350, 20), (3, 16, 400, 18),
    (4, 20, 750,  4), (4, 26, 580,  2), (4, 29, 450,  5), (4, 30, 480,  3), (4, 34, 900,  0.5), (4, 35, 1100, 2),
    (5, 21, 2400, 4), (5, 28, 4300, 5), (5, 37, 1800, 2),
    (6, 22, 3200, 3), (6, 24, 280,  5), (6, 25, 1500, 0.5), (6, 33, 950, 4), (6, 36, 2200, 0.3),
    (6, 12, 480,  24), (6, 13, 330, 20), (6, 14, 1400, 12), (6, 15, 1100, 6), (6, 16, 380, 18), (6, 17, 1600, 4),
    (7, 19, 4200, 3), (7, 23, 1600, 2), (7, 27, 200,  30),
    (8, 18, 650,  10), (8, 31, 120, 10), (8, 32, 350,  20);

-- ============================================================
-- NOTIFICACIONES
-- ============================================================

INSERT INTO notificacion (id, restaurante_id, fecha, descripcion, resuelta) VALUES
    (1, 1, NOW() - INTERVAL '2 hours',    'Stock bajo de lechuga (debajo del mínimo)',   FALSE),
    (2, 1, NOW() - INTERVAL '1 hour',     'Lote de fideos spaghetti VENCIDO',            FALSE),
    (3, 1, NOW() - INTERVAL '30 minutes', 'Lote de huevos vence HOY',                    FALSE),
    (4, 1, NOW() - INTERVAL '4 hours',    'Pedido #3 de Distribuidora Central recibido', TRUE);

-- ============================================================
-- RESET DE SECUENCIAS
-- ============================================================

SELECT setval('ubicacion_id_seq',         (SELECT MAX(id) FROM ubicacion));
SELECT setval('restaurante_id_seq',       (SELECT MAX(id) FROM restaurante));
SELECT setval('carta_id_seq',             (SELECT MAX(id) FROM carta));
SELECT setval('turno_laboral_id_seq',     (SELECT MAX(id) FROM turno_laboral));
SELECT setval('empleado_id_seq',          (SELECT MAX(id) FROM empleado));
SELECT setval('dimension_mesa_id_seq',    (SELECT MAX(id) FROM dimension_mesa));
SELECT setval('grilla_id_seq',            (SELECT MAX(id) FROM grilla));
SELECT setval('mesa_id_seq',              (SELECT MAX(id) FROM mesa));
SELECT setval('reserva_id_seq',           (SELECT MAX(id) FROM reserva));
SELECT setval('llamado_id_seq',           (SELECT MAX(id) FROM llamado));
SELECT setval('fila_virtual_id_seq',      (SELECT MAX(id) FROM fila_virtual));
SELECT setval('turno_fila_id_seq',        (SELECT MAX(id) FROM turno_fila));
SELECT setval('articulo_id_seq',          (SELECT MAX(id) FROM articulo));
SELECT setval('categoria_insumo_id_seq',  (SELECT MAX(id) FROM categoria_insumo));
SELECT setval('lote_id_seq',              (SELECT MAX(id) FROM lote));
SELECT setval('comanda_id_seq',           (SELECT MAX(id) FROM comanda));
SELECT setval('cierre_id_seq',            (SELECT MAX(id) FROM cierre));
SELECT setval('pago_id_seq',              (SELECT MAX(id) FROM pago));
SELECT setval('pedido_id_seq',            (SELECT MAX(id) FROM pedido));
SELECT setval('proveedor_id_seq',         (SELECT MAX(id) FROM proveedor));
SELECT setval('notificacion_id_seq',      (SELECT MAX(id) FROM notificacion));
SELECT setval('bodega_id_seq',            (SELECT MAX(id) FROM bodega));
SELECT setval('articulo_comanda_id_seq',  (SELECT MAX(id) FROM articulo_comanda));
SELECT setval('familia_tipografica_id_seq',        (SELECT MAX(id) FROM familia_tipografica));

COMMIT;
