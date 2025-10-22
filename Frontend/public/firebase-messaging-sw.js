importScripts('https://www.gstatic.com/firebasejs/10.12.0/firebase-app-compat.js');
importScripts('https://www.gstatic.com/firebasejs/10.12.0/firebase-messaging-compat.js');

firebase.initializeApp({
  apiKey: "AIzaSyDgRFMv4_zMGFsQGwyb7AaX6tLFo2fxzmQ",
  authDomain: "to-do-inteligente-fcm.firebaseapp.com",
  projectId: "to-do-inteligente-fcm",
  storageBucket: "to-do-inteligente-fcm.firebasestorage.app",
  messagingSenderId: "1000603442629",
  appId: "1:1000603442629:web:87dec49ed1d349bd263fac",
  measurementId: "G-05HMRQQX0H"
});

//Iniciar messaging
const messaging = firebase.messaging();

//Muestra las notificaciones cuando la app esta cerrada
messaging.onBackgroundMessage((payload) => {
    console.log('[firebase-messaging-sw.js] Recibido en segundo plano:', payload);
    const notificationTitle = payload.notification.title;
    const notificationOptions = {
        body: payload.notification.body,
        icon: '/notificaciones.png' //Aqui se maneja el icono
    }
    self.registration.showNotification(notificationTitle, notificationOptions);
});