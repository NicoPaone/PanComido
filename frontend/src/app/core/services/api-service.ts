import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ApiService {

  private http = inject(HttpClient);

  private baseUrl = environment.apiUrl;

  get<T>(endpoint: string, params?: HttpParams): Observable<T>{
    return this.http.get<T>(`${this.baseUrl}/${endpoint}`, { params });
  }

  post<T>(endpoint: string, body: unknown, headers?:HttpHeaders): Observable<T>{
    return this.http.post<T>(`${this.baseUrl}/${endpoint}`, body, { headers });
  }
  
  put<T>(endpoint: string, body?: unknown): Observable<T> {
    return this.http.put<T>(`${this.baseUrl}/${endpoint}`, body);
  }
  
  patch<T>(endpoint: string, body?: unknown): Observable<T> {
    return this.http.patch<T>(`${this.baseUrl}/${endpoint}`, body);
  }
  
  delete<T>(endpoint: string): Observable<T> {
    return this.http.delete<T>(`${this.baseUrl}/${endpoint}`);
  }
}
