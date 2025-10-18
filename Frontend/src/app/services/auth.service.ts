import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { ILoginDTO } from "../models/login.model";
import { IRegisterDTO } from "../models/register.model";
import { Observable, tap } from "rxjs";

@Injectable({
  providedIn: "root"
})

export class AuthService {
  private apiUrlLogin = "https://localhost:7035/api/Users/login"; //endpoint de login
  private apiUrlRegister = "https://localhost:7035/api/Users/register"; //endpoint del registro

  constructor(private http: HttpClient) { }

  login(credentials: ILoginDTO): Observable<any> {
    return this.http.post(this.apiUrlLogin, credentials).pipe(
      tap((res: any) => {
        //El backend aqui retorna el token JWT
        localStorage.setItem("token", res.token);
      })
    )
  }

  register(userData: IRegisterDTO) {
    return this.http.post(this.apiUrlRegister, userData);
  }

  getToken(): string | null {
    return localStorage.getItem("token");
  }

  logout(): void {
    localStorage.removeItem("token");
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

 }
