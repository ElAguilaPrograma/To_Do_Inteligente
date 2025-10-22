import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { AppModule } from './app/app.module';

if ('serviceWorker' in navigator){
  navigator.serviceWorker.register('/firebase-messaging-sw.js')
  .then(reg => console.log("Service Worker FCM registrado", reg))
  .catch(err => console.log("Error al registrar el Service Worker FCM", err));
}

platformBrowserDynamic().bootstrapModule(AppModule, {
  ngZoneEventCoalescing: true,
})
  .catch(err => console.error(err));