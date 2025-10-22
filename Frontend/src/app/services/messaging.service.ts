import { Injectable } from "@angular/core";
import { BehaviorSubject } from "rxjs";
import { Messaging, getToken, onMessage } from '@angular/fire/messaging';
import { environment } from '../../environments/environment';
import { AngularFireMessaging } from "@angular/fire/compat/messaging";
import { HttpClient } from "@angular/common/http";
import { AuthService } from "./auth.service";

@Injectable({
    providedIn: 'root'
})
export class MessagingService {
    currentMessage = new BehaviorSubject<any>(null);

    constructor(private afMessaging: AngularFireMessaging, private http: HttpClient, private authService: AuthService) {
        this.listen();
    }

    requestPermission() {
    this.afMessaging.requestToken.subscribe({
      next: (token) => {
        console.log('FCM Token:', token);
        Notification.requestPermission().then((result) => {
            console.log("Estado del permiso: ", result)
            if (result === "granted"){
                this.sendTokenToServer(token);
            }
        })
      },
      error: (err) => {
        console.error('No se pudo obtener el permiso de notificación.', err);
      }
    });
  }

  listen() {
        this.afMessaging.messages.subscribe((payload: any) => {
            console.log('📨 Mensaje recibido:', payload);

            if (payload.notification) {
            const { title, body } = payload.notification;

            // Mostrar una notificación nativa del navegador
            new Notification(title, {
                body,
                icon: '/notificaciones.png'
            });
            }
        });
    }

  private sendTokenToServer(token: string | null) {

    if (!token) {
    console.warn("Token es null, no se enviará al servidor.");
    return;
    }

    const jwt = this.authService.getToken();

    // Verificar que el JWT no sea nulo
    if (!jwt) {
        console.error("No hay token JWT disponible");
        return;
    }

    this.http.post(
        "https://localhost:7035/api/Notification/register-device",
        { deviceToken: token, platform: "web" },
        { headers: { Authorization: `Bearer ${jwt}` } }
    ).subscribe({
        next: () => console.log("Token enviado al backend"),
        error: (err) => console.error("Error enviando el token")
    });
  }

}