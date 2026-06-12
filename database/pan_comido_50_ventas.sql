-- ============================================================
-- PAN COMIDO - Script para generar 50 ventas (Comandas y Pagos) 
-- Distribuidas en los últimos 3 meses
-- ============================================================

DO $$
DECLARE
    i INT;
    v_mesa_id INT;
    v_fecha TIMESTAMP;
    v_pago_id INT;
    v_comanda_id INT;
    v_total DECIMAL;
    v_cant_articulos INT;
    v_articulo_id INT;
    v_precio_venta DECIMAL;
    v_cantidad INT;
    v_j INT;
BEGIN
    -- Aseguramos que la semilla aleatoria sea variada
    FOR i IN 1..50 LOOP
        -- 1. Generar una fecha aleatoria entre hoy y hace 90 días
        v_fecha := NOW() - (random() * 90 || ' days')::interval;
        
        -- 2. Seleccionar una mesa aleatoria (del 1 al 31 que existen)
        v_mesa_id := floor(random() * 31 + 1)::int;
        
        -- 3. Insertar el Pago (primero con total 0, lo calcularemos al sumar artículos)
        -- metodo_pago_id va de 1 a 4 (Efectivo, Tarjeta, Transferencia, MP)
        INSERT INTO pago (cierre_id, metodo_pago_id, total)
        VALUES (NULL, floor(random() * 4 + 1)::int, 0)
        RETURNING id INTO v_pago_id;
        
        -- 4. Insertar la Comanda vinculada al pago.
        -- estado_comanda_id = 4 (Finalizada)
        -- restaurante_id = 1
        INSERT INTO comanda (mesa_id, pago_id, restaurante_id, estado_comanda_id, cant_comensales, hora_inicio, hora_fin)
        VALUES (
            v_mesa_id, 
            v_pago_id, 
            1, 
            4, 
            floor(random() * 4 + 1)::int, 
            v_fecha, 
            v_fecha + (random() * 2 || ' hours')::interval
        )
        RETURNING id INTO v_comanda_id;
        
        v_total := 0;
        
        -- 5. Insertar entre 1 y 5 artículos vendibles por comanda
        v_cant_articulos := floor(random() * 5 + 1)::int;
        FOR v_j IN 1..v_cant_articulos LOOP
            -- Seleccionar artículo vendible aleatorio (platos del 1 al 11 o bebidas del 12 al 17)
            v_articulo_id := floor(random() * 17 + 1)::int;
            
            -- Obtener el precio real del artículo (priorizando precio promocional si tiene, sino el precio venta final)
            SELECT COALESCE(precio_promocional, precio_venta_final) INTO v_precio_venta
            FROM articulo WHERE id = v_articulo_id;
            
            -- Generar cantidad pedida de este artículo (1 a 3)
            v_cantidad := floor(random() * 3 + 1)::int;
            
            -- Insertar el artículo en la comanda
            INSERT INTO articulo_comanda (comanda_id, articulo_id, cantidad, entregado)
            VALUES (v_comanda_id, v_articulo_id, v_cantidad, TRUE);
            
            -- Sumar al total del pago
            v_total := v_total + (v_precio_venta * v_cantidad);
        END LOOP;
        
        -- 6. Actualizar el pago con el total final real calculado sumando todos los artículos
        UPDATE pago SET total = v_total WHERE id = v_pago_id;
        
    END LOOP;
END $$;
