import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ILoginDTO } from '../../models/login.model';
import { NgForm } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  loginData: ILoginDTO = { email: "", password: "" }

  constructor(private authService: AuthService, private router: Router) { }

  onLogin(form: NgForm) {
    if (form.valid) {
      this.authService.login(this.loginData).subscribe({
        next: (res) => {
          console.log("Login Exitoso", res);
          this.router.navigate(["/home"]);
        },
        error: (err) => console.log("Error al iniciar sesión", err)
      })
    }
  }
}
