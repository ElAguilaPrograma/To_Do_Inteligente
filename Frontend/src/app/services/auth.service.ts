import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { ILoginDTO } from "../models/login.model";

@Injectable({
  providedIn: "root"
})

export class AuthService {
  private apiUrlLogin = "https://localhost:7035/api/Users/login"; //endpoint de login

  constructor(private http: HttpClient) { }

  login(credentials: ILoginDTO) {
    return this.http.post(this.apiUrlLogin, credentials);
  }
}
