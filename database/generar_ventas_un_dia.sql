DO $$ 
DECLARE
    v_restaurante_id INT;
    v_mesa_id INT;
    v_estado_comanda_id INT;
    v_metodo_pago_id INT;
    v_estado_pago_id INT;
    v_comanda_id INT;
    v_articulo_id INT;
    v_fecha_base DATE := CURRENT_DATE;
    v_hora_inicio TIMESTAMP;
    v_hora_fin TIMESTAMP;
    v_total_pago NUMERIC;
    v_precio_articulo NUMERIC;
BEGIN
    -- Obtener IDs base necesarios para asegurar que no falle por FKs
    
    -- 1. Obtener estado de comanda (preferentemente uno que signifique Cerrado/Pagado)
    SELECT id INTO v_estado_comanda_id FROM estado_comanda ORDER BY id DESC LIMIT 1;
    
    -- 2. Obtener un estado de pago
    SELECT id INTO v_estado_pago_id FROM estado_pago ORDER BY id DESC LIMIT 1;
    
    -- Loop para generar 50 ventas en un solo día
    FOR i IN 1..50 LOOP
        -- Obtener un restaurante y una mesa al azar para cada venta
        SELECT r.id, m.id 
        INTO v_restaurante_id, v_mesa_id
        FROM restaurante r
        JOIN grilla g ON g.restaurante_id = r.id
        JOIN mesa m ON m.grilla_id = g.id
        ORDER BY random()
        LIMIT 1;

        -- Si no hay mesas, saltar
        IF v_restaurante_id IS NULL OR v_mesa_id IS NULL THEN
            CONTINUE;
        END IF;

        -- Horarios aleatorios entre las 10:00 y las 23:59 del día de hoy
        v_hora_inicio := v_fecha_base + (random() * interval '14 hours' + interval '10 hours');
        v_hora_fin := v_hora_inicio + interval '1 hour' + (random() * interval '30 minutes');

        -- 1. Insertar Comanda
        INSERT INTO comanda (mesa_id, restaurante_id, estado_comanda_id, cant_comensales, hora_inicio, hora_fin, hora_ultimo_cambio_estado)
        VALUES (v_mesa_id, v_restaurante_id, v_estado_comanda_id, (random() * 4 + 1)::int, v_hora_inicio, v_hora_fin, v_hora_fin)
        RETURNING id INTO v_comanda_id;

        v_total_pago := 0;

        -- 2. Insertar Artículos de Comanda (entre 1 y 4 artículos)
        FOR j IN 1..((random() * 3 + 1)::int) LOOP
            -- Buscar un artículo al azar del restaurante
            SELECT id, COALESCE(precio_venta_final, 1000) 
            INTO v_articulo_id, v_precio_articulo
            FROM articulo 
            WHERE restaurante_id = v_restaurante_id AND eliminado = false 
            ORDER BY random() 
            LIMIT 1;
            
            IF FOUND THEN
                INSERT INTO articulo_comanda (comanda_id, articulo_id, cantidad, entregado, observaciones_generales, nombre_comensal)
                VALUES (v_comanda_id, v_articulo_id, (random() * 2 + 1)::int, true, NULL, 'Comensal ' || j);
                
                v_total_pago := v_total_pago + v_precio_articulo;
            END IF;
        END LOOP;

        -- Asegurar que el total no sea 0 si no se encontraron artículos
        IF v_total_pago = 0 THEN
            v_total_pago := (random() * 5000 + 1000)::numeric;
        END IF;

        -- 3. Insertar Pago
        -- Obtener un método de pago al azar
        SELECT id INTO v_metodo_pago_id FROM metodo_de_pago ORDER BY random() LIMIT 1;
        
        INSERT INTO pago (comanda_id, metodo_pago_id, estado_pago_id, external_reference, total)
        VALUES (v_comanda_id, v_metodo_pago_id, v_estado_pago_id, 'TEST-1DIA-' || v_comanda_id, v_total_pago);
        
    END LOOP;
END $$;
