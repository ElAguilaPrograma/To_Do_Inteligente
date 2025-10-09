import { Component, EventEmitter, Output } from '@angular/core';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-header',
  standalone: false,
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent {
  @Output() toggleSidebar = new EventEmitter<void>();

  constructor(private authService: AuthService) { }

  getToken(): void {
    const token = this.authService.getToken();
    console.log("Token: ", token);
  }

  LogOut(): void {
    this.authService.logout();
    console.log("Saliendo....")
    window.location.reload();
  }
}
