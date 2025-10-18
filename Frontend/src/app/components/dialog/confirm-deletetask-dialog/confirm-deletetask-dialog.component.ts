import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-confirm-deletetask-dialog',
  standalone: false,
  templateUrl: './confirm-deletetask-dialog.component.html',
  styleUrl: './confirm-deletetask-dialog.component.css'
})
export class ConfirmDeletetaskDialogComponent {
  constructor(public dialogRef: MatDialogRef<ConfirmDeletetaskDialogComponent>) {}

  onConfirm(): void{
    this.dialogRef.close(true);
  }

  onCancel(): void{
    this.dialogRef.close(false);
  }

}
