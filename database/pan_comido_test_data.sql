-- ============================================================
-- PAN COMIDO - Datos de prueba completos (PostgreSQL / Supabase)
-- Grupo 5 - "No se deJava"
-- Ejecutar DESPUÉS del script DDL y del script SEED
-- ============================================================
-- IDs de articulo:
--   1-11:  Platos
--   12-17: Bebidas (artículo vendible + insumo, sin tabla bebida propia)
--   18-37: Ingredientes simples (artículo + insumo + ingrediente)
--   38-41: Ingredientes preparados (artículo + insumo + ingrediente + ingrediente_preparado)
-- ============================================================

BEGIN;

-- ============================================================
-- RESTAURANTE
-- ============================================================

INSERT INTO ubicacion (id, direccion, ciudad, codigo_postal) VALUES
    (1, 'Av. Corrientes 1234', 'CABA', '1043');

INSERT INTO restaurante (id, direccion_id, nombre, imagen, color_principal, color_secundario, texto_principal, texto_secundario) VALUES
    (1, 1, 'Pan Comido', '/img/logo-pan-comido.png', '#FBAC28', '#C5172E', 'Fredoka One', 'Nunito');

INSERT INTO carta (id, restaurante_id) VALUES
    (1, 1);

-- ============================================================
-- TURNOS LABORALES
-- ============================================================

INSERT INTO turno_laboral (id, restaurante_id, horario_laboral_inicio, horario_laboral_fin) VALUES
    (1, 1, '08:00', '16:00'),
    (2, 1, '16:00', '00:00');

-- ============================================================
-- EMPLEADOS Y ROLES
-- ============================================================

INSERT INTO empleado (id, restaurante_id, nombre, email, contrasena, estado) VALUES
    (1, 1, 'Carlos López',     'carlos@pancomido.com',    '$2b$10$hash_simulado_gerente',  'activo'),
    (2, 1, 'Cocina',            'cocina@pancomido.com',    '$2b$10$hash_simulado_cocina1',  'activo'),
    (3, 1, 'Ana Rodríguez',    'ana@pancomido.com',       '$2b$10$hash_simulado_mozo1',    'activo'),
    (4, 1, 'Pedro Martínez',   'pedro@pancomido.com',     '$2b$10$hash_simulado_mozo2',    'activo'),
    (5, 1, 'Laura Fernández',  'laura@pancomido.com',     '$2b$10$hash_simulado_mozo3',    'activo'),
    (6, 1, 'Diego Sánchez',    'diego@pancomido.com',     '$2b$10$hash_simulado_inactivo', 'inactivo');

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
    (5, '/img/mesa-redonda-4.png',     'redonda');

INSERT INTO grilla (id, restaurante_id, cant_columnas, cant_filas) VALUES
    (1, 1, 8, 6);

-- estado_mesa: 1=Disponible, 2=Ocupada, 3=Reservada, 4=Deshabilitada
INSERT INTO mesa (id, grilla_id, estado_mesa_id, dimension_mesa_id, posicion_x_inicio, posicion_x_fin, posicion_y_inicio, posicion_y_fin, numero, codigo_invitacion, cant_personas_max) VALUES
    (1, 1, 1, 1, 1, 2, 1, 2, 1, 'INV-001', 2),
    (2, 1, 1, 2, 3, 4, 1, 2, 2, 'INV-002', 4),
    (3, 1, 2, 2, 5, 6, 1, 2, 3, 'INV-003', 4),
    (4, 1, 2, 3, 1, 3, 3, 4, 4, 'INV-004', 6),
    (5, 1, 3, 5, 4, 5, 3, 4, 5, 'INV-005', 4),
    (6, 1, 1, 4, 6, 8, 3, 4, 6, 'INV-006', 8),
    (7, 1, 4, 2, 1, 2, 5, 6, 7, NULL,      4),
    (8, 1, 1, 1, 3, 4, 5, 6, 8, 'INV-008', 2);

INSERT INTO mozo_mesa (mozo_id, mesa_id) VALUES
    (3, 1), (3, 2), (3, 3),
    (4, 4), (4, 5), (4, 6),
    (5, 8);

-- ============================================================
-- RESERVAS
-- ============================================================

INSERT INTO reserva (id, mesa_id, cant_comensales, nombre_titular, fecha, horario, tel_contacto) VALUES
    (1, 5, 3, 'Roberto Gómez',    CURRENT_DATE, '20:30', '1155667788'),
    (2, 2, 2, 'Lucía Herrera',     CURRENT_DATE + 1, '21:00', '1144556677'),
    (3, 6, 7, 'Familia Rodríguez', CURRENT_DATE + 2, '13:00', '1133445566');

-- ============================================================
-- ARTÍCULOS
-- ============================================================

INSERT INTO articulo (id, carta_id, restaurante_id, nombre, descripcion, precio_venta_final, precio_ganancia, precio_promocional) VALUES
    -- === PLATOS (vendibles, en carta) ===
    (1,  1, 1, 'Pizza Muzzarella',         'Pizza clásica con mozzarella y salsa de tomate',           4500,  2800, NULL),
    (2,  1, 1, 'Pizza Napolitana',          'Pizza con tomate, mozzarella y ajo',                       5000,  3200, NULL),
    (3,  1, 1, 'Milanesa Napolitana',       'Milanesa de pollo con jamón, queso y salsa',               6500,  4000, NULL),
    (4,  1, 1, 'Hamburguesa Clásica',       'Carne vacuna, lechuga, tomate, cebolla',                   5500,  3500, NULL),
    (5,  1, 1, 'Ensalada César',            'Lechuga, pollo grillado, croutons, parmesano',             4000,  2500, 3500),
    (6,  1, 1, 'Bife de Chorizo',           'Bife de chorizo a la parrilla con guarnición',             8500,  5500, NULL),
    (7,  1, 1, 'Spaghetti Bolognesa',       'Pasta con salsa bolognesa casera',                         5000,  3000, NULL),
    (8,  1, 1, 'Empanadas de Carne (x3)',   'Empanadas de carne cortada a cuchillo',                    3000,  1800, NULL),
    (9,  1, 1, 'Papas Fritas',              'Porción de papas fritas',                                  2500,  1500, NULL),
    (10, 1, 1, 'Ensalada Mixta',            'Lechuga, tomate, cebolla, huevo',                          2000,  1200, NULL),
    (11, 1, 1, 'Wok de Pollo y Verduras',   'Pollo salteado con pimiento, cebolla y salsa de soja',     5500,  3300, 4800),

    -- === BEBIDAS (vendibles, en carta, son insumo con categoria_insumo tipo_aplica=2) ===
    (12, 1, 1, 'Coca-Cola 500ml',           'Gaseosa línea Coca-Cola',         1500, 900,  NULL),
    (13, 1, 1, 'Agua Mineral 500ml',        'Agua mineral sin gas',            1000, 600,  NULL),
    (14, 1, 1, 'Cerveza Artesanal IPA',     'Pinta de cerveza artesanal IPA',  2500, 1500, NULL),
    (15, 1, 1, 'Vino Malbec (copa)',        'Copa de Malbec Reserva',          2000, 1200, NULL),
    (16, 1, 1, 'Sprite 500ml',              'Gaseosa Sprite',                  1500, 900,  NULL),
    (17, 1, 1, 'Fernet con Coca',           'Fernet Branca con Coca-Cola',     2500, 1500, NULL),

    -- === INGREDIENTES SIMPLES (no vendibles, sin carta) ===
    (18, NULL, 1, 'Harina 000',             NULL, NULL, NULL, NULL),
    (19, NULL, 1, 'Mozzarella',             NULL, NULL, NULL, NULL),
    (20, NULL, 1, 'Tomate perita',          NULL, NULL, NULL, NULL),
    (21, NULL, 1, 'Pechuga de pollo',       NULL, NULL, NULL, NULL),
    (22, NULL, 1, 'Aceite de oliva',        NULL, NULL, NULL, NULL),
    (23, NULL, 1, 'Crema de leche',         NULL, NULL, NULL, NULL),
    (24, NULL, 1, 'Sal',                    NULL, NULL, NULL, NULL),
    (25, NULL, 1, 'Pimienta',              NULL, NULL, NULL, NULL),
    (26, NULL, 1, 'Lechuga',               NULL, NULL, NULL, NULL),
    (27, NULL, 1, 'Huevos',                NULL, NULL, NULL, NULL),
    (28, NULL, 1, 'Carne vacuna (bife)',    NULL, NULL, NULL, NULL),
    (29, NULL, 1, 'Papa',                  NULL, NULL, NULL, NULL),
    (30, NULL, 1, 'Cebolla',              NULL, NULL, NULL, NULL),
    (31, NULL, 1, 'Ajo',                  NULL, NULL, NULL, NULL),
    (32, NULL, 1, 'Pan de hamburguesa',    NULL, NULL, NULL, NULL),
    (33, NULL, 1, 'Fideos secos',          NULL, NULL, NULL, NULL),
    (34, NULL, 1, 'Albahaca',             NULL, NULL, NULL, NULL),
    (35, NULL, 1, 'Pimiento rojo',        NULL, NULL, NULL, NULL),
    (36, NULL, 1, 'Orégano',              NULL, NULL, NULL, NULL),
    (37, NULL, 1, 'Jamón cocido',          NULL, NULL, NULL, NULL),

    -- === INGREDIENTES PREPARADOS (no vendibles, sin carta) ===
    (38, NULL, 1, 'Salsa de tomate casera', NULL, NULL, NULL, NULL),
    (39, NULL, 1, 'Masa de pizza',          NULL, NULL, NULL, NULL),
    (40, NULL, 1, 'Masa de empanada',       NULL, NULL, NULL, NULL),
    (41, NULL, 1, 'Salsa bechamel',         NULL, NULL, NULL, NULL);

-- ============================================================
-- CONFIGURACIÓN DE ARTÍCULOS
-- ============================================================

INSERT INTO articulo_configuracion_articulo (articulo_id, configuracion_articulo_id) VALUES
    (1, 1), (1, 2),
    (2, 1), (2, 2),
    (3, 1), (3, 2),
    (4, 1), (4, 2),
    (5, 1), (5, 2),
    (6, 1), (6, 2),
    (7, 1),          -- Spaghetti: vendible pero NO visible (fideos vencidos)
    (8, 1), (8, 2),
    (9, 1), (9, 2),
    (10, 1), (10, 2),
    (11, 1), (11, 2),
    (12, 1), (12, 2),
    (13, 1), (13, 2),
    (14, 1), (14, 2),
    (15, 1), (15, 2),
    (16, 1), (16, 2),
    (17, 1), (17, 2);

-- ============================================================
-- PLATOS (subtipo de Articulo)
-- ============================================================
-- tipo_plato: 1=Pasta, 2=Minuta, 3=Pizza, 4=Parrilla, 5=Sandwich,
--   6=Ensalada, 7=Mariscos, 8=Wok y Salteado, 9=Tarta y Empanada
-- categoria_plato: 1=Entrada, 2=Principal, 3=Postre, 4=Guarnición

INSERT INTO plato (id_articulo, tipo_plato_id, categoria_plato_id, tiempo_preparacion_base, destacado, sugerencia) VALUES
    (1,  3, 2, 20, TRUE,  'Ideal para compartir'),
    (2,  3, 2, 25, FALSE, NULL),
    (3,  2, 2, 25, TRUE,  'Nuestro plato estrella'),
    (4,  5, 2, 15, FALSE, NULL),
    (5,  6, 1, 10, FALSE, 'Liviana, ideal como entrada'),
    (6,  4, 2, 30, TRUE,  'Corte premium'),
    (7,  1, 2, 15, FALSE, NULL),
    (8,  9, 1, 20, FALSE, 'Recién salidas del horno'),
    (9,  2, 4, 10, FALSE, NULL),
    (10, 6, 1, 5,  FALSE, NULL),
    (11, 8, 2, 15, FALSE, 'Opción ligera y sabrosa');

-- ============================================================
-- INSUMOS (subtipo de Articulo) — ahora con categoria_insumo_id y unidad_medida_id
-- ============================================================
-- categoria_insumo: 1=Fruta, 2=Verdura, 3=Carne, 4=Lácteos, 5=Cereales,
--   6=Pescado y Mariscos, 7=Huevos, 8=Condimentos y Especias, 9=Aceites y Grasas,
--   10=Legumbres, 11=Harinas y Panificados, 12=Con alcohol, 13=Sin alcohol
-- unidad_medida: 1=Kg, 2=Gr, 3=Lt, 4=Ml, 5=Unidad, 6=Porción

INSERT INTO insumo (id_articulo, categoria_insumo_id, unidad_medida_id, stock_minimo) VALUES
    -- Bebidas (tipo_aplica = 2)
    (12, 13, 5, 5),      -- Coca-Cola → Sin alcohol, Unidad
    (13, 13, 5, 3),      -- Agua mineral → Sin alcohol, Unidad
    (14, 12, 5, 2),      -- Cerveza IPA → Con alcohol, Unidad
    (15, 12, 5, 2),      -- Vino Malbec → Con alcohol, Unidad
    (16, 13, 5, 3),      -- Sprite → Sin alcohol, Unidad
    (17, 12, 5, 1),      -- Fernet → Con alcohol, Unidad

    -- Ingredientes simples (tipo_aplica = 1)
    (18, 11, 1, 2),      -- Harina 000 → Harinas y Panificados, Kg
    (19, 4,  1, 1),      -- Mozzarella → Lácteos, Kg
    (20, 2,  1, 0.5),    -- Tomate perita → Verdura, Kg
    (21, 3,  1, 1),      -- Pechuga de pollo → Carne, Kg
    (22, 9,  3, 0.5),    -- Aceite de oliva → Aceites y Grasas, Lt
    (23, 4,  3, 0.3),    -- Crema de leche → Lácteos, Lt
    (24, 8,  1, 2),      -- Sal → Condimentos y Especias, Kg
    (25, 8,  2, 1),      -- Pimienta → Condimentos y Especias, Gr
    (26, 2,  1, 0.5),    -- Lechuga → Verdura, Kg
    (27, 7,  5, 0.3),    -- Huevos → Huevos, Unidad
    (28, 3,  1, 1),      -- Carne vacuna → Carne, Kg
    (29, 2,  1, 0.5),    -- Papa → Verdura, Kg
    (30, 2,  1, 0.3),    -- Cebolla → Verdura, Kg
    (31, 8,  5, 1),      -- Ajo → Condimentos y Especias, Unidad
    (32, 11, 5, 0.5),    -- Pan de hamburguesa → Harinas y Panificados, Unidad
    (33, 5,  1, 1),      -- Fideos secos → Cereales, Kg
    (34, 2,  2, 1),      -- Albahaca → Verdura, Gr
    (35, 2,  1, 0.3),    -- Pimiento rojo → Verdura, Kg
    (36, 8,  2, 1),      -- Orégano → Condimentos y Especias, Gr
    (37, 3,  1, 0.3),    -- Jamón cocido → Carne, Kg

    -- Ingredientes preparados (tipo_aplica = 1)
    (38, 2,  3, 0.5),    -- Salsa de tomate casera → Verdura, Lt
    (39, 11, 6, 0.3),    -- Masa de pizza → Harinas y Panificados, Porción
    (40, 11, 6, 0.2),    -- Masa de empanada → Harinas y Panificados, Porción
    (41, 4,  3, 0.3);    -- Salsa bechamel → Lácteos, Lt

-- ============================================================
-- INGREDIENTES (subtipo de Insumo — tabla marcadora)
-- ============================================================
-- Solo los que participan en recetas (plato_ingrediente) o composiciones (ingrediente_preparado)
-- Las bebidas (12-17) NO van acá porque no son ingredientes

INSERT INTO ingrediente (id_insumo) VALUES
    (18), (19), (20), (21), (22), (23), (24), (25),
    (26), (27), (28), (29), (30), (31), (32), (33),
    (34), (35), (36), (37),
    (38), (39), (40), (41);

-- ============================================================
-- INGREDIENTES PREPARADOS (subtipo de Ingrediente)
-- ============================================================

INSERT INTO ingrediente_preparado (id_ingrediente) VALUES
    (38), (39), (40), (41);

-- Composición de ingredientes preparados
-- Salsa de tomate casera (38) = tomate(20) + cebolla(30) + ajo(31) + aceite(22) + sal(24) + albahaca(34)
INSERT INTO ingrediente_ingrediente_preparado (ingrediente_id, ingrediente_preparado_id) VALUES
    (20, 38), (30, 38), (31, 38), (22, 38), (24, 38), (34, 38);

-- Masa de pizza (39) = harina(18) + sal(24) + aceite(22)
INSERT INTO ingrediente_ingrediente_preparado (ingrediente_id, ingrediente_preparado_id) VALUES
    (18, 39), (24, 39), (22, 39);

-- Masa de empanada (40) = harina(18) + sal(24) + aceite(22)
INSERT INTO ingrediente_ingrediente_preparado (ingrediente_id, ingrediente_preparado_id) VALUES
    (18, 40), (24, 40), (22, 40);

-- Salsa bechamel (41) = harina(18) + crema(23) + sal(24) + pimienta(25)
INSERT INTO ingrediente_ingrediente_preparado (ingrediente_id, ingrediente_preparado_id) VALUES
    (18, 41), (23, 41), (24, 41), (25, 41);

-- ============================================================
-- RESTRICCIONES DE PLATOS
-- ============================================================

INSERT INTO restriccion_plato (restriccion_id, plato_id) VALUES
    (2, 1),   -- Pizza Muzza → Vegetariano
    (2, 9),   -- Papas fritas → Vegetariano
    (3, 9),   -- Papas fritas → Sin TACC
    (1, 10),  -- Ensalada Mixta → Vegano
    (2, 10),  -- Ensalada Mixta → Vegetariano
    (3, 6);   -- Bife de Chorizo → Sin TACC

-- ============================================================
-- RECETAS: PLATO <-> INGREDIENTE
-- ============================================================

-- Pizza Muzzarella (1)
INSERT INTO plato_ingrediente (plato_id, ingrediente_id, opcional) VALUES
    (1, 39, FALSE), (1, 38, FALSE), (1, 19, FALSE), (1, 36, TRUE);

-- Pizza Napolitana (2)
INSERT INTO plato_ingrediente (plato_id, ingrediente_id, opcional) VALUES
    (2, 39, FALSE), (2, 38, FALSE), (2, 19, FALSE), (2, 31, FALSE), (2, 20, FALSE);

-- Milanesa Napolitana (3)
INSERT INTO plato_ingrediente (plato_id, ingrediente_id, opcional) VALUES
    (3, 21, FALSE), (3, 27, FALSE), (3, 18, FALSE), (3, 37, FALSE), (3, 19, FALSE), (3, 38, FALSE);

-- Hamburguesa Clásica (4)
INSERT INTO plato_ingrediente (plato_id, ingrediente_id, opcional) VALUES
    (4, 28, FALSE), (4, 32, FALSE), (4, 26, FALSE), (4, 20, FALSE), (4, 30, TRUE);

-- Ensalada César (5)
INSERT INTO plato_ingrediente (plato_id, ingrediente_id, opcional) VALUES
    (5, 26, FALSE), (5, 21, FALSE), (5, 27, FALSE), (5, 22, FALSE), (5, 24, FALSE), (5, 25, TRUE);

-- Bife de Chorizo (6)
INSERT INTO plato_ingrediente (plato_id, ingrediente_id, opcional) VALUES
    (6, 28, FALSE), (6, 24, FALSE), (6, 25, FALSE), (6, 22, TRUE);

-- Spaghetti Bolognesa (7)
INSERT INTO plato_ingrediente (plato_id, ingrediente_id, opcional) VALUES
    (7, 33, FALSE), (7, 38, FALSE), (7, 28, FALSE), (7, 30, FALSE), (7, 31, FALSE);

-- Empanadas de Carne (8)
INSERT INTO plato_ingrediente (plato_id, ingrediente_id, opcional) VALUES
    (8, 40, FALSE), (8, 28, FALSE), (8, 30, FALSE), (8, 27, FALSE), (8, 35, TRUE);

-- Papas Fritas (9)
INSERT INTO plato_ingrediente (plato_id, ingrediente_id, opcional) VALUES
    (9, 29, FALSE), (9, 22, FALSE), (9, 24, FALSE);

-- Ensalada Mixta (10)
INSERT INTO plato_ingrediente (plato_id, ingrediente_id, opcional) VALUES
    (10, 26, FALSE), (10, 20, FALSE), (10, 30, TRUE), (10, 27, FALSE), (10, 22, FALSE), (10, 24, FALSE);

-- Wok de Pollo (11)
INSERT INTO plato_ingrediente (plato_id, ingrediente_id, opcional) VALUES
    (11, 21, FALSE), (11, 35, FALSE), (11, 30, FALSE), (11, 22, FALSE), (11, 24, FALSE), (11, 25, TRUE);

-- ============================================================
-- BODEGAS Y LOTES
-- ============================================================

INSERT INTO bodega (id, restaurante_id, tipo_bodega_id, nombre) VALUES
    (1, 1, 1, 'Almacén principal'),
    (2, 1, 2, 'Heladera cocina'),
    (3, 1, 3, 'Freezer');

INSERT INTO lote (id, insumo_id, bodega_id, nombre, cantidad, fecha_adquisicion, fecha_vencimiento) VALUES
    -- Almacén (bodega 1): secos, aceites, bebidas
    (1,  18, 1, 'Harina 000 - Lote Mar/26',          10,   '2026-03-01', '2026-06-15'),
    (2,  22, 1, 'Aceite oliva - Lote Feb/26',          5,   '2026-02-01', '2026-08-01'),
    (3,  24, 1, 'Sal fina - Lote Ene/26',              3,   '2026-01-10', '2026-12-01'),
    (4,  25, 1, 'Pimienta negra - Lote Ene/26',        0.5, '2026-01-10', '2026-12-01'),
    (5,  32, 1, 'Pan burger - Lote May/26',             20,  '2026-05-25', '2026-07-20'),
    (6,  33, 1, 'Fideos spaghetti - Lote Ene/26',      3,   '2026-01-15', '2026-05-25'),  -- VENCIDO
    (7,  36, 1, 'Orégano seco - Lote Mar/26',          0.3, '2026-03-01', '2026-08-15'),
    (8,  12, 1, 'Coca-Cola 500ml - Pack May/26',       24,  '2026-05-01', NULL),
    (9,  13, 1, 'Agua mineral 500ml - Pack May/26',    20,  '2026-05-01', NULL),
    (10, 14, 1, 'Cerveza IPA - Lote Abr/26',           12,  '2026-04-10', '2027-03-01'),
    (11, 15, 1, 'Malbec Reserva - Lote Mar/26',        6,   '2026-03-01', '2027-01-15'),
    (12, 16, 1, 'Sprite 500ml - Pack May/26',           18,  '2026-05-01', NULL),
    (13, 17, 1, 'Fernet 750ml - Lote Abr/26',          4,   '2026-04-15', '2027-06-01'),

    -- Heladera cocina (bodega 2): frescos, lácteos, preparados
    (14, 19, 2, 'Mozzarella - Lote Abr/26',            3,   '2026-04-15', '2026-07-01'),
    (15, 19, 2, 'Mozzarella - Lote May/26',            2,   '2026-05-20', '2026-07-20'),
    (16, 20, 2, 'Tomate perita - Lote May/26',         4,   '2026-05-15', '2026-06-03'),
    (17, 21, 2, 'Pechuga - Lote May/26 (fresca)',      1.5, '2026-05-22', '2026-06-10'),
    (18, 23, 2, 'Crema - Lote May/26',                 2,   '2026-05-18', '2026-06-20'),
    (19, 26, 2, 'Lechuga - Lote May/26',               1,   '2026-05-25', '2026-06-01'),
    (20, 27, 2, 'Huevos - Lote May/26',                12,  '2026-05-20', '2026-05-27'),  -- Vence HOY
    (21, 29, 2, 'Papa - Lote May/26',                  8,   '2026-05-15', '2026-07-15'),
    (22, 30, 2, 'Cebolla - Lote May/26',               4,   '2026-05-10', '2026-06-08'),
    (23, 31, 2, 'Ajo - Lote Abr/26',                   1,   '2026-04-01', '2026-09-01'),
    (24, 34, 2, 'Albahaca fresca - Lote May/26',       0.2, '2026-05-26', '2026-06-30'),
    (25, 35, 2, 'Pimiento rojo - Lote May/26',         2,   '2026-05-20', '2026-07-10'),
    (26, 37, 2, 'Jamón cocido - Lote May/26',          2,   '2026-05-22', '2026-06-12'),
    (27, 38, 2, 'Salsa tomate - Prep May/26',          3,   '2026-05-25', '2026-06-05'),
    (28, 39, 2, 'Masa pizza - Prep May/26',            8,   '2026-05-26', '2026-06-04'),
    (29, 40, 2, 'Masa empanada - Prep May/26',         15,  '2026-05-26', '2026-06-06'),
    (30, 41, 2, 'Bechamel - Prep May/26',              2,   '2026-05-25', '2026-06-07'),

    -- Freezer (bodega 3): carnes congeladas
    (31, 21, 3, 'Pechuga - Lote May/26 (congelada)',   1.5, '2026-05-22', '2026-06-10'),
    (32, 28, 3, 'Bife angosto - Lote May/26',          5,   '2026-05-24', '2026-06-25');

-- ============================================================
-- MÉTODOS DE PAGO HABILITADOS
-- ============================================================

INSERT INTO metodo_de_pago_restaurante (restaurante_id, metodo_de_pago_id, habilitado) VALUES
    (1, 1, TRUE), (1, 2, TRUE), (1, 3, TRUE), (1, 4, TRUE);

-- ============================================================
-- CIERRES Y PAGOS
-- ============================================================

INSERT INTO cierre (id, restaurante_id, turno_laboral_id, diferencia, sobrante) VALUES
    (1, 1, 1, -150.00, 0);

INSERT INTO pago (id, cierre_id, metodo_pago_id, total) VALUES
    (1, 1, 1, 28500.00),
    (2, 1, 2, 15200.00),
    (3, 1, 4, 8300.00);

-- ============================================================
-- COMANDAS
-- ============================================================

INSERT INTO comanda (id, mesa_id, pago_id, restaurante_id, estado_comanda_id, cant_comensales, hora_inicio, hora_fin) VALUES
    (1, 3, NULL, 1, 2, 3, NOW() - INTERVAL '25 minutes', NULL),
    (2, 4, NULL, 1, 1, 5, NOW() - INTERVAL '5 minutes', NULL),
    (3, 1, 1,   1, 3, 2, NOW() - INTERVAL '3 hours', NOW() - INTERVAL '2 hours');

INSERT INTO articulo_comanda (comanda_id, articulo_id, cantidad, entregado, observaciones_ingrediente, observaciones_generales) VALUES
    -- Comanda 1 (mesa 3, en preparación): algunos entregados, con observaciones
    (1, 1,  1, FALSE, 'Sin orégano',          NULL),
    (1, 12, 2, TRUE,  NULL,                    NULL),                         -- Coca-Cola ya entregada
    (1, 6,  1, FALSE, NULL,                    'Cocción a punto'),
    (1, 9,  1, TRUE,  NULL,                    NULL),                         -- Papas ya entregadas

    -- Comanda 2 (mesa 4, nueva): nada entregado
    (2, 8,  2, FALSE, NULL,                    NULL),
    (2, 3,  1, FALSE, 'Sin jamón',             'Que no toque la ensalada'),
    (2, 4,  2, FALSE, 'Sin cebolla',           NULL),
    (2, 5,  1, FALSE, NULL,                    'Aderezo aparte'),
    (2, 14, 2, FALSE, NULL,                    NULL),
    (2, 15, 1, FALSE, NULL,                    NULL),
    (2, 13, 2, FALSE, NULL,                    NULL),

    -- Comanda 3 (cerrada): todo entregado
    (3, 2,  1, TRUE,  NULL,                    NULL),
    (3, 11, 1, TRUE,  'Sin pimienta',          NULL),
    (3, 16, 2, TRUE,  NULL,                    NULL);

-- ============================================================
-- LLAMADOS
-- ============================================================

INSERT INTO llamado (id, mozo_id, gerente_id, categoria_llamado_id, descripcion, resuelto) VALUES
    (1, 3, NULL, 1, 'Mesa 3 pidió hielo para las bebidas',      FALSE),
    (2, 3, NULL, 6, 'Mesa 3 pidió más pan',                     TRUE),
    (3, 4, 1,   3, 'Mesa 4 quiere hablar con el encargado',     FALSE);

-- ============================================================
-- FILA VIRTUAL
-- ============================================================

INSERT INTO fila_virtual (id, restaurante_id) VALUES (1, 1);

INSERT INTO turno_fila (id, fila_virtual_id, numero) VALUES
    (1, 1, 1), (2, 1, 2), (3, 1, 3);

-- ============================================================
-- PROVEEDORES Y PEDIDOS
-- ============================================================

INSERT INTO proveedor (id, restaurante_id, nombre, numero_telefono_wsp) VALUES
    (1, 1, 'Verdulería Don José',    '1122334455'),
    (2, 1, 'Carnicería El Gaucho',    '1133445566'),
    (3, 1, 'Distribuidora Central',   '1144556677'),
    (4, 1, 'Lácteos del Campo',       '1155667788'),
    (5, 1, 'Panadería San Martín',    '1166778899');

-- N:M CategoriaInsumo <-> Proveedor (reemplaza a categoria_proveedor)
-- categoria_insumo: 2=Verdura, 3=Carne, 4=Lácteos, 9=Aceites, 11=Harinas, 12=Con alcohol, 13=Sin alcohol
INSERT INTO categoria_insumo_proveedor (categoria_insumo_id, proveedor_id) VALUES
    (1, 1),   -- Fruta → Verdulería Don José
    (2, 1),   -- Verdura → Verdulería Don José
    (3, 2),   -- Carne → Carnicería El Gaucho
    (5, 3),   -- Cereales → Distribuidora Central
    (9, 3),   -- Aceites y Grasas → Distribuidora Central
    (12, 3),  -- Con alcohol → Distribuidora Central
    (13, 3),  -- Sin alcohol → Distribuidora Central
    (4, 4),   -- Lácteos → Lácteos del Campo
    (11, 5);  -- Harinas y Panificados → Panadería San Martín

INSERT INTO pedido (id, proveedor_id, estado_pedido_id, fecha) VALUES
    (1, 1, 1, CURRENT_DATE),
    (2, 2, 2, CURRENT_DATE - 1),
    (3, 3, 3, CURRENT_DATE - 3);

INSERT INTO pedido_insumo (pedido_id, insumo_id, precio_compra, cantidad) VALUES
    (1, 20, 800,  5),    (1, 26, 600,  2),    (1, 30, 500,  3),
    (2, 21, 2500, 5),    (2, 28, 4500, 3),
    (3, 12, 500,  24),   (3, 13, 350,  20),   (3, 16, 400,  18);

-- ============================================================
-- NOTIFICACIONES
-- ============================================================

INSERT INTO notificacion (id, restaurante_id, fecha, descripcion, resuelta) VALUES
    (1, 1, NOW() - INTERVAL '2 hours',    'Stock bajo de lechuga (debajo del mínimo)',       FALSE),
    (2, 1, NOW() - INTERVAL '1 hour',     'Lote de fideos spaghetti VENCIDO',                FALSE),
    (3, 1, NOW() - INTERVAL '30 minutes', 'Lote de huevos vence HOY',                        FALSE),
    (4, 1, NOW() - INTERVAL '4 hours',    'Pedido #3 de Distribuidora Central recibido',     TRUE);

-- ============================================================
-- RESET DE SECUENCIAS
-- ============================================================

SELECT setval('ubicacion_id_seq', (SELECT MAX(id) FROM ubicacion));
SELECT setval('restaurante_id_seq', (SELECT MAX(id) FROM restaurante));
SELECT setval('carta_id_seq', (SELECT MAX(id) FROM carta));
SELECT setval('turno_laboral_id_seq', (SELECT MAX(id) FROM turno_laboral));
SELECT setval('empleado_id_seq', (SELECT MAX(id) FROM empleado));
SELECT setval('dimension_mesa_id_seq', (SELECT MAX(id) FROM dimension_mesa));
SELECT setval('grilla_id_seq', (SELECT MAX(id) FROM grilla));
SELECT setval('mesa_id_seq', (SELECT MAX(id) FROM mesa));
SELECT setval('reserva_id_seq', (SELECT MAX(id) FROM reserva));
SELECT setval('llamado_id_seq', (SELECT MAX(id) FROM llamado));
SELECT setval('fila_virtual_id_seq', (SELECT MAX(id) FROM fila_virtual));
SELECT setval('turno_fila_id_seq', (SELECT MAX(id) FROM turno_fila));
SELECT setval('articulo_id_seq', (SELECT MAX(id) FROM articulo));
SELECT setval('categoria_insumo_id_seq', (SELECT MAX(id) FROM categoria_insumo));
SELECT setval('lote_id_seq', (SELECT MAX(id) FROM lote));
SELECT setval('comanda_id_seq', (SELECT MAX(id) FROM comanda));
SELECT setval('cierre_id_seq', (SELECT MAX(id) FROM cierre));
SELECT setval('pago_id_seq', (SELECT MAX(id) FROM pago));
SELECT setval('pedido_id_seq', (SELECT MAX(id) FROM pedido));
SELECT setval('proveedor_id_seq', (SELECT MAX(id) FROM proveedor));
SELECT setval('notificacion_id_seq', (SELECT MAX(id) FROM notificacion));
SELECT setval('bodega_id_seq', (SELECT MAX(id) FROM bodega));
SELECT setval('articulo_comanda_id_seq', (SELECT MAX(id) FROM articulo_comanda));

COMMIT;
