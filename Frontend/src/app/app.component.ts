import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.css',
})
export class AppComponent {
  title = 'Frontend';

  /*
  constructor(private messagingService: MessagingService) {}

  ngOnInit(): void {
      this.messagingService.requestPermission();
  }
  */
}
