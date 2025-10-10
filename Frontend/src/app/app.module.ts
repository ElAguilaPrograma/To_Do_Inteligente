import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './components/login/login.component';
import { FormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS, HttpClient, HttpClientModule } from '@angular/common/http';
import { RegisterComponent } from './components/register/register.component';
import { TasksComponent } from './components/tasks/tasks.component';
import { AuthInterceptor } from './services/auth.interceptor';
import { LayoutComponent } from './components/layout/layout.component';
import { HeaderComponent } from './components/header/header.component';
import { SidebarComponent } from './components/sidebar/sidebar.component';
import { SettingsComponent } from './components/settings/settings.component';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialogModule } from '@angular/material/dialog';
import { ConfirmLogoutDialogComponent } from './components/dialog/confirm-logout-dialog/confirm-logout-dialog.component';
import {MatButtonModule} from '@angular/material/button';
import { ConfirmDeletetaskDialogComponent } from './components/dialog/confirm-deletetask-dialog/confirm-deletetask-dialog.component';


@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    TasksComponent,
    LayoutComponent,
    HeaderComponent,
    SidebarComponent,
    SettingsComponent,
    ConfirmLogoutDialogComponent,
    ConfirmDeletetaskDialogComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule,
    MatTooltipModule,
    MatDialogModule,
    MatButtonModule
],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
