import { HttpInterceptorFn, HttpResponse } from "@angular/common/http";
import { environment } from "../../../environments/environment.development";
//import { handleStockMock } from "./handlers/producto-stock";
import { handleMesasMock } from "./handlers/mesas.mock-handlers";

export const mockInterceptor: HttpInterceptorFn = (req, next) => {
   if(!environment.useMock){
      return next(req);
   }
   const url = req.url;

   // Enrutamiento de mocks
  // if(url.includes('/stock-mercaderia')){
   //   return handleStockMock(req, next);
  // }


   if(url.includes('/mesas')){
      return handleMesasMock(req, next);
   }

   return next(req);
}
