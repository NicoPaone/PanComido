# PanComido - Frontend

Sistema de gestión para restaurante desarrollado con Angular.

---

### 1. Gestión de Estado y Datos
```typescript
// Ejemplo de patrón StateService con Signals (Próxima implementación)
protected readonly user = computed(() => this._user()); 
```

### 2. Estándares de Desarrollo
- **Standalone Components**: Arquitectura sin NgModules para una gestión de dependencias granular.
- **Signals**: Uso de reactividad fina para la gestión de estados locales y globales.
- **Clean Code**: Priorización de legibilidad y tipado estricto en TypeScript.

### 3. Convenciones de Nomenclatura
- **Archivos**: `kebab-case` (ej: `stock-list.ts`).
- **Clases**: `PascalCase` (ej: `StockList`).
- **Selectores**: Prefijo `app-` (ej: `app-stock-list`).

### 4. Organización Modular
- **core/**: Servicios singleton, interceptores y modelos compartidos globales (se cargan una vez en el root).
- **features/**: Módulos organizados por dominio funcional (auth, cocina, comensal, gerente, mozo). Implementación de lazy loading.
- **shared/**: Componentes de UI reutilizables (botones, modales, buscadores) y componentes transversales sin lógica de negocio pesada.
- **layouts/**: Estructuras de contenedores principales para diferentes roles de usuario.

### 5. Guía de Ejecución
```bash
npm install   # Instalación de dependencias
npm start     # Servidor de desarrollo en localhost:4200
```

---
*Documentación técnica de referencia para el equipo de PanComido.*
