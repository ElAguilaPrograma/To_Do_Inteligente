import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { TasksComponent } from './components/tasks/tasks.component';
import { AuthGuard } from './services/auth.guard';
import { LayoutComponent } from './components/layout/layout.component';
import { SettingsComponent } from './components/settings/settings.component';

const routes: Routes = [
  { path: "", redirectTo: "login", pathMatch: "full" }, //Ruta por defecto
  { path: "login", component: LoginComponent },
  { path: "register", component: RegisterComponent },

  {
    path: "",
    component: LayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: "home", component: TasksComponent },
      { path: "settings", component: SettingsComponent}
    ]
  },

  { path: "**", redirectTo: "login" }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
