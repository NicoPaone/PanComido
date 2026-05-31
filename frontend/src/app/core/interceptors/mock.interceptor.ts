import { HttpInterceptorFn } from "@angular/common/http";
import { environment } from "../../../environments/environment.development";
import { handleStockMock } from "./handlers/producto-stock";
import { handlePlatoMock } from "./handlers/plato";
import { handleProveedorMock } from "./handlers/proveedor";
import { handleMesasMock } from "./handlers/mesas.mock-handlers";

export const mockInterceptor: HttpInterceptorFn = (req, next) => {
   if(!environment.useMock){
      return next(req);
   }
   const url = req.url;
   if(url.includes('/mesas')){
      return handleMesasMock(req, next);
   }
   if (url.includes('/stock-mercaderia')) {
    return handleStockMock(req, next);
   }
   if (url.includes('/platos')) {
    return handlePlatoMock(req, next);
   }

   if (url.includes('/Proveedor') || url.includes('/proveedores')) {
    return handleProveedorMock(req, next);
   }
   return next(req);
}

