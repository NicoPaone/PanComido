-- ============================================================
-- PAN COMIDO - Script de datos seed (PostgreSQL / Supabase)
-- Grupo 5 - "No se deJava"
-- Ejecutar DESPUÉS del script DDL
-- ============================================================
BEGIN;

INSERT INTO estado_mesa (descripcion) VALUES
    ('Disponible'), ('Ocupada'), ('Reservada'), ('Deshabilitada');

INSERT INTO estado_comanda (descripcion) VALUES
    ('Nueva'), ('En preparación'), ('En espera'), ('Finalizada'), ('Abierta');

INSERT INTO estado_pedido (descripcion) VALUES
    ('Pendiente'), ('Enviado'), ('Recibido');

INSERT INTO categoria_plato (descripcion) VALUES
    ('Entrada'), ('Principal'), ('Postre'), ('Guarnición');

INSERT INTO tipo_plato (descripcion) VALUES
    ('Pasta'), ('Minuta'), ('Pizza'), ('Parrilla'), ('Sandwich'),
    ('Ensalada'), ('Mariscos'), ('Wok y Salteado'), ('Tarta y Empanada');

INSERT INTO restriccion (descripcion) VALUES
    ('Vegano'), ('Vegetariano'), ('Sin TACC');

INSERT INTO categoria_llamado (descripcion) VALUES
    ('Hielo'), ('Sal'), ('General'), ('Servilleta'), ('Condimentos'), ('Panera'), ('Pago');

INSERT INTO configuracion_articulo (descripcion) VALUES
    ('Es vendible'), ('Visible en carta');

INSERT INTO categoria_insumo (descripcion, tipo_aplica) VALUES
    ('Fruta', 1),                   -- 1
    ('Verdura', 1),                 -- 2
    ('Carne', 1),                   -- 3
    ('Lácteos', 1),                 -- 4
    ('Cereales', 1),                -- 5
    ('Pescado y Mariscos', 1),      -- 6
    ('Huevos', 1),                  -- 7
    ('Condimentos y Especias', 1),  -- 8
    ('Aceites y Grasas', 1),        -- 9
    ('Legumbres', 1),               -- 10
    ('Harinas y Panificados', 1),   -- 11
    ('Con alcohol', 2),             -- 12
    ('Sin alcohol', 2);             -- 13

INSERT INTO unidad_medida (nombre) VALUES
    ('Kg'), ('Gr'), ('Lt'), ('Ml'), ('Unidad'), ('Porción');

INSERT INTO tipo_bodega (descripcion) VALUES
    ('Almacén'), ('Cámara de frío'), ('Cámara de congelados');

INSERT INTO metodo_de_pago (descripcion) VALUES
    ('Efectivo'), ('Tarjeta'), ('Transferencia'), ('Mercado Pago');

INSERT INTO estado_pago (descripcion) VALUES
    ('Pendiente'), ('Confirmado'), ('Rechazado');

-- Familias tipográficas predefinidas
-- Moderna: sans-serif limpias y minimalistas
-- Clásica: serif elegantes con sans-serif neutras
-- Rústica: display con carácter y tipografías legibles
INSERT INTO familia_tipografica (categoria, tipografia_titulo, tipografia_cuerpo) VALUES
    ('Moderna',  'Montserrat',         'Lato'),
    ('Moderna',  'Poppins',            'Roboto'),
    ('Moderna',  'Oswald',             'Open Sans'),
    ('Clásica',  'Playfair Display',   'Raleway'),
    ('Clásica',  'Cormorant Garamond', 'Nunito'),
    ('Clásica',  'Helvetica',          'Georgia'),
    ('Rústica',  'Fredoka One',        'Source Sans Pro'),
    ('Rústica',  'Righteous',          'Karla'),
    ('Rústica',  'Abril Fatface',      'Merriweather');

COMMIT;
