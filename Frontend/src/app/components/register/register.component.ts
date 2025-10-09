import { Component } from '@angular/core';
import { IRegisterDTO } from '../../models/register.model';
import { AuthService } from '../../services/auth.service';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-register',
  standalone: false,
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  registerData: IRegisterDTO = { userName: "", email: "", password: "", phone: "" }

  constructor(private authService: AuthService) { }

  onRegister(form: NgForm) {
    if (form.valid) {
      const phone = this.registerData.phone;

      if (!/^\d{10}$/.test(phone)) {
        alert("El número de teléfono debe tener 10 dígitos.");
        return;
      }

      this.authService.register(this.registerData).subscribe({
        next: (res) => console.log("Registrado con exito", res),
        error: (err) => console.log("Error al registrar", err)
      })
    }
    else {
      alert("Por favor, complete todos los campos correctamente");
    }
  }
}
