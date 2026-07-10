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
       -- Distribuimos las 50 ventas inteligentemente para tener datos en todos los filtros
        IF i <= 5 THEN
            -- 5 ventas en las últimas 24 horas (Filtro: Hoy)
            v_fecha := NOW() - (random() * 1 || ' days')::interval;
        ELSIF i <= 15 THEN
            -- 10 ventas en los últimos 3 días (Filtro: 3 días)
            v_fecha := NOW() - (random() * 3 || ' days')::interval;
        ELSIF i <= 25 THEN
            -- 10 ventas en los últimos 7 días (Filtro: 7 días)
            v_fecha := NOW() - (random() * 7 || ' days')::interval;
        ELSIF i <= 35 THEN
            -- 10 ventas en los últimos 30 días (Filtro: 1 mes)
            v_fecha := NOW() - (random() * 30 || ' days')::interval;
        ELSE
            -- 15 ventas perdidas en los últimos 90 días (Filtro: Histórico/Año)
            v_fecha := NOW() - (random() * 90 || ' days')::interval;
        END IF;
        
        -- 2. Seleccionar una mesa aleatoria solo entre las que tienen mozo (1 al 8)
        v_mesa_id := floor(random() * 8 + 1)::int;
        
        -- 3. Insertar la Comanda vinculada al pago.
        -- estado_comanda_id = 4 (Finalizada)
        -- restaurante_id = 1
        INSERT INTO comanda (mesa_id, restaurante_id, estado_comanda_id, cant_comensales, hora_inicio, hora_fin)
        VALUES (
            v_mesa_id, 
            1, 
            4, 
            floor(random() * 4 + 1)::int, 
            v_fecha, 
            v_fecha + (random() * 2 || ' hours')::interval
        )
        RETURNING id INTO v_comanda_id;

        -- 4. Insertar el Pago (primero con total 0, lo calcularemos al sumar artículos)
        -- metodo_pago_id va de 1 a 4 (Efectivo, Tarjeta, Transferencia, MP)
        INSERT INTO pago (comanda_id, cierre_id, metodo_pago_id, estado_pago_id, total)
        VALUES (v_comanda_id, NULL, floor(random() * 4 + 1)::int, 2, 0)
        RETURNING id INTO v_pago_id;
        
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


-- ============================================================
-- Generador Aleatorio de Encuestas de Satisfacción
-- ============================================================
DO $$
DECLARE
    rec RECORD;
    v_lugar INT;
    v_comida INT;
    v_mozo INT;
    v_cant_encuestas INT;
    v_k INT;
BEGIN
        -- Recorremos TODAS las comandas finalizadas (estado = 4)
    FOR rec IN SELECT id, hora_fin FROM comanda WHERE estado_comanda_id = 4 LOOP
        
        -- Generamos entre 0 y 3 encuestas aleatorias para esta misma comanda
        -- (Si sale 0, significa que nadie en la mesa llenó la encuesta)
        v_cant_encuestas := floor(random() * 4)::int; 
        
        FOR v_k IN 1..v_cant_encuestas LOOP
            
            -- Generamos puntuaciones aleatorias del 1 al 5.
            v_lugar := floor(random() * 3 + 3)::int; 
            v_comida := floor(random() * 3 + 3)::int;
            v_mozo := floor(random() * 5 + 1)::int;
            
            -- Insertamos la encuesta. Como está dentro de este nuevo FOR, 
            -- puede ejecutarse varias veces para el mismo rec.id
            INSERT INTO encuesta_satisfaccion (comanda_id, puntuacion_lugar, puntuacion_comida, puntuacion_mozo, fecha)
            VALUES (
                rec.id, 
                v_lugar, 
                v_comida, 
                v_mozo, 
                rec.hora_fin + (random() * 15 || ' minutes')::interval
            );
            
        END LOOP;
    END LOOP;
END $$;
