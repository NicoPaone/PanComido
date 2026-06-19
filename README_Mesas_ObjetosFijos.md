# Implementación de Objetos Fijos (Escenarios, Barras) en el Mapa de Mesas

Este documento contiene la estrategia y el código exacto necesario para agregar soporte de objetos fijos interactuables en el editor del mapa, reutilizando toda la arquitectura actual sin romper la API para la aplicación de los mozos.

## Estrategia: Single Table Inheritance (STI)
En lugar de crear tablas y controladores nuevos que rompan los endpoints actuales (`GET /mesa` y `PUT /mesa/mapa`), extendemos la entidad `Mesa` actual para que soporte funcionar como "Objeto Fijo". Esto permite:
- **Cero duplicación** de la lógica de colisiones en el frontend.
- **Cero duplicación** de la lógica de Drag and Drop.
- **Compatibilidad total** con el guardado masivo de las posiciones del mapa.

---

## 1. Cambios en Backend (C#)

### Entidades y Dominio
Archivos: 
- `PanComido.Presentacion/Persistencia/Entidades/Mesa.cs`
- `PanComido.Dominio/Entidades/MesaMapaDominio.cs`

Agregar las siguientes propiedades a ambas clases:
```csharp
public int TipoElemento { get; set; } = 1; // 1 = Mesa, 2 = ObjetoReserva/Fijo
public string? Color { get; set; }
public string? TextoObjeto { get; set; }
```

### DTOs
Archivos:
- `PanComido.Presentacion/DTOs/Mesas/MesaResponseDto.cs`
- `PanComido.Presentacion/DTOs/Mesas/GuardarMesaRequestDto.cs`

Agregar a ambas clases:
```csharp
public int TipoElemento { get; set; } = 1; 
public string? Color { get; set; }
public string? TextoObjeto { get; set; }
```

> **Nota:** Recordá agregar estas nuevas columnas a tu base de datos (y al mapping de EF Core si corresponde).

---

## 2. Cambios en Frontend (Angular)

### Interfaz Base
Archivo: `core/models/domain/mesa.ts`
```typescript
export interface Mesa {
  // ... propiedades existentes
  tipoElemento?: number; // 1 = Mesa, 2 = Objeto Fijo
  color?: string;
  textoObjeto?: string;
}
```

### State Service
Archivo: `features/mesas/services/mesa.state.ts`
Agregar el método para instanciar el objeto fijo en el modo editor:
```typescript
agregarObjetoFijo(): void {
  const idNegativo = -(Math.floor(Math.random() * 1000000) + 1);
  const nuevoObjeto: Mesa = {
    id: idNegativo,
    codigoInvitacion: '',
    numeroMesa: 0, 
    cantidadPersonasMax: 0,
    estadoMesa: EstadoMesa.Disponible,
    dimensionMesa: { id: 0, forma: FormaMesa.Cuadrada }, // Dummy
    posicionXInicio: 15, posicionXFin: 215, // 200px ancho
    posicionYInicio: 15, posicionYFin: 115, // 100px alto
    tipoElemento: 2,
    color: '#34495e',
    textoObjeto: 'Escenario'
  };

  this.lectura.updateMesas(m => [...m, nuevoObjeto]);
}
```

### Vista del Mapa (HTML)
Archivo: `features/mesas/pages/mapa-mesas/mapa-mesas.html`

**A. Botón para el menú del editor:**
```html
<button *ngIf="state.isEditorMode()" (click)="state.agregarObjetoFijo()" class="btn-agregar">
  Agregar Objeto Fijo
</button>
```

**B. Renderizado Condicional en la Grilla:**
Reemplazar el `*ngFor` actual de las mesas por un contenedor iterador que diferencie entre tipos:
```html
<ng-container *ngFor="let item of state.mesas()">
  
  <!-- Render de Mesa normal -->
  <app-mesa-item 
    *ngIf="item.tipoElemento !== 2" 
    [mesa]="item"
    cdkDrag 
    (cdkDragEnded)="onDragEnded(item.id, $event)">
  </app-mesa-item>

  <!-- Render de Objeto Fijo -->
  <div *ngIf="item.tipoElemento === 2"
       class="objeto-fijo"
       cdkDrag
       [cdkDragDisabled]="!state.isEditorMode()"
       [style.width.px]="item.posicionXFin - item.posicionXInicio"
       [style.height.px]="item.posicionYFin - item.posicionYInicio"
       [style.background-color]="item.color"
       [style.transform]="'translate3d(' + item.posicionXInicio + 'px, ' + item.posicionYInicio + 'px, 0)'"
       (cdkDragEnded)="onDragEnded(item.id, $event)">
    <span style="color: white; font-weight: bold; pointer-events: none;">{{ item.textoObjeto }}</span>
  </div>

</ng-container>
```

---
### Filtrado para la vista de los Mozos
Para que a los mozos no les aparezca un escenario en su lista de "Mis Mesas", simplemente asegurate de filtrar el array en los componentes o vistas correspondientes (ej. en la app móvil):
```typescript
this.mesasParaMostrar = this.state.mesas().filter(m => m.tipoElemento !== 2);
```
