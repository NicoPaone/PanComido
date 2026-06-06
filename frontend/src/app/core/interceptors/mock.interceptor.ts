import { HttpInterceptorFn } from "@angular/common/http";
import { environment } from "../../../environments/environment";
import { handleStockMock } from "../../infra/mocks/handlers/producto-stock.mock-handler";
import { handlePlatoMock } from "../../infra/mocks/handlers/plato.mock-handler";
import { handleProveedorMock } from "../../infra/mocks/handlers/proveedor.mock-handler";

/**
 * Interceptor de mocks — solo actúa cuando environment.useMock === true.
 *
 * Convención de rutas:
 *   /insumo              → handleStockMock (CRUD de insumos/stock)
 *   /carta               → handlePlatoMock (CRUD de platos/artículos)
 *   /plato               → handlePlatoMock (crear plato)
 *   /proveedor           → handleProveedorMock (CRUD de proveedores)
 *   /pedido-proveedor    → handleProveedorMock (pedidos a proveedores)
 */
export const mockInterceptor: HttpInterceptorFn = (req, next) => {
   if (!environment.useMock) {
      return next(req);
   }

   const url = req.url.toLowerCase();

   // Insumos / stock-mercadería
   if (url.includes('/insumo')) {
      return handleStockMock(req, next);
   }

   // Platos / carta
   if (url.includes('/carta') || url.includes('/plato')) {
      return handlePlatoMock(req, next);
   }

   // Proveedores y pedidos
   if (url.includes('/proveedor') || url.includes('/pedido-proveedor')) {
      return handleProveedorMock(req, next);
   }

   return next(req);
}
