import { HttpInterceptorFn } from "@angular/common/http";
import { inject } from "@angular/core";
import { Router } from "@angular/router";
import { catchError, throwError } from "rxjs";

const TOKEN_KEY = 'pancomido_token'

export const jwtInterceptor: HttpInterceptorFn = (req,next) => {
  const token = localStorage.getItem(TOKEN_KEY);
  const router = inject(Router);

  if(token){
    req = req.clone({
      setHeaders: { Authorization: `Bearer ${token}`}
    });
  }
  return next(req).pipe(
    catchError(error => {
      if(error.status=== 401){
        localStorage.removeItem(TOKEN_KEY);
        router.navigate(['/login'])
      }
      return throwError(()=> error);

    })
  )
}