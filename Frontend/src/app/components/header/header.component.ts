import { Component, EventEmitter, Output, inject } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { ConfirmLogoutDialogComponent } from '../dialog/confirm-logout-dialog/confirm-logout-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { Route, Router } from '@angular/router';

@Component({
  selector: 'app-header',
  standalone: false,
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent {
  @Output() toggleSidebar = new EventEmitter<void>();

  constructor(
    private authService: AuthService,
    private dialog: MatDialog,
    private router: Router
  ) { }

  getToken(): void {
    const token = this.authService.getToken();
    console.log("Token: ", token);
  }

  LogOut(): void {
    const dialogRef = this.dialog.open(ConfirmLogoutDialogComponent, {
      width: "350px",
      disableClose: true,
      enterAnimationDuration: "200ms",
      exitAnimationDuration: "150ms"
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.authService.logout();
        window.location.reload();
      }
    })
  }
}
