import { HttpInterceptorFn, HttpResponse } from "@angular/common/http";
import { environment } from "../../../environments/environment.development";
import { handleStockMock } from "./handlers/producto-stock";




export const mockInterceptor: HttpInterceptorFn = (req, next) => {
   if(!environment.useMock){
      return next(req);
   }

   const url = req.url;

   //vamos a usarlo para enrutar los mocks
   if(url.includes('/stock-mercaderia')){
      return handleStockMock(req, next);
   }
  

   return next(req);
}