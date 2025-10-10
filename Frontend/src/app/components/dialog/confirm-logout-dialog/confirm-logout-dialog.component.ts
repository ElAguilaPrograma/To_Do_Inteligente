import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-confirm-logout-dialog',
  standalone: false,
  templateUrl: './confirm-logout-dialog.component.html',
  styleUrl: './confirm-logout-dialog.component.css'
})
export class ConfirmLogoutDialogComponent {
  constructor(public dialogRef: MatDialogRef<ConfirmLogoutDialogComponent>) {}

  onConfirm(): void{
    this.dialogRef.close(true);
  }

  onCancel(): void{
    this.dialogRef.close(false);
  }

}
