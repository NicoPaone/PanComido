import { HttpInterceptorFn } from "@angular/common/http";
import { environment } from "../../../environments/environment.development";
import { handleStockMock } from "./handlers/producto-stock";
import { handleAvisosMock } from "./handlers/avisos.mock";

export const mockInterceptor: HttpInterceptorFn = (req, next) => {
   if (!environment.useMock) {
      return next(req);
   }
   const url = req.url;
   // if(url.includes('/mesas')){
   //    return handleMesasMock(req, next);
   // }
   if (url.includes('/stock-mercaderia')) {
      return handleStockMock(req, next);
   }
   if (url.includes('/avisos') || url.endsWith('/Insumo')) {
     return handleAvisosMock(req, next);
   }
   return next(req);
}
