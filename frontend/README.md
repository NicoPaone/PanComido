# PanComido - Frontend 🍽️

---

## 🏗️ Arquitectura del Proyecto

Usamos una arquitectura basada en **Features** (Funcionalidades). La idea es que el código esté cerca de donde se usa. Nada de tener archivos perdidos por cualquier lado. Separamos las responsabilidades para que, si algo rompe, sepas exactamente dónde meter mano.

### Estructura de Carpetas

La posta está en `src/app`. Así es como separamos los tantos:

*   **`core/`**: Es el corazón del sistema. Acá va todo lo que es **singleton** (una sola instancia) o global.
    *   `models/`: Interfaces y tipos de TypeScript que se usan en todo el proyecto.
    *   `services/`: Servicios globales (facades, interceptors, etc.).
*   **`features/`**: Acá vive la lógica de negocio. Cada carpeta es un "módulo" funcional.
    *   `auth/`, `gerente/`, `cocina/`, `mozo/`, `comensal/`.
    *   Cada feature tiene sus propias `pages`, `components` y su archivo de `routes.ts`.
*   **`shared/`**: Todo lo que se puede reutilizar en distintas partes de la aplicación.
    *   `ui/`: Componentes básicos y puros (Botones, Inputs, Modals, Buscadores). Seguimos una onda parecida al **Atomic Design** para los componentes de UI.
    *   `components/`: Componentes compartidos que tienen un poco más de contexto (Header, Sidebar).
*   **`layouts/`**: Definimos las estructuras visuales grandes. Por ejemplo, el layout para el staff (con sidebar) y el layout para el comensal.

---

## 🛠️ Estándares de Desarrollo

### Componentes Standalone
Acá no usamos `NgModules` gigantes que son un quilombo de mantener. Todos los componentes nuevos son **Standalone**. Esto nos da más claridad en las dependencias y hace que el árbol de componentes sea más fácil de razonar.

### Convenciones de Nombres
*   **Archivos**: `nombre-del-componente.ts`, `nombre.model.ts`.
*   **Clases**: `PascalCase` (ej: `ModificarCartaComponent`).
*   **Selectores**: `app-nombre-del-componente`.

---

## 🚀 Próximos Pasos (Hoja de Ruta)

A medida que el proyecto crezca, vamos a ir metiendo agregando:

1.  **State Management**: Vamos a usar un patrón de `StateService` basado en **Signals**. Nada de estados perdidos en los componentes; la verdad siempre va a estar en el servicio.
2.  **Service Layer**: Los componentes no deberían saber de dónde vienen los datos. Vamos a centralizar las llamadas a la API en servicios dedicados.
3.  **Guards**: Seguridad ante todo. Vamos a implementar Guards para que nadie entre a donde no debe (ej: un mozo no debería poder ver el dashboard del gerente).

