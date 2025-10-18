import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { ITask } from '../../models/tasks.models/task.model';
import { TaskService } from '../../services/task/task.service';
import { NgForm } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDeletetaskDialogComponent } from '../dialog/confirm-deletetask-dialog/confirm-deletetask-dialog.component';

@Component({
  selector: 'app-tasks',
  standalone: false,
  templateUrl: './tasks.component.html',
  styleUrl: './tasks.component.css',
})
export class TasksComponent {
  tasks: ITask[] = [];
  isLoading = false;
  errorMessage = "";
  newTask = false;
  showSideBar = false;
  selectedTask: ITask = {title: "", description: "", isCompleted: false}
  taskData: ITask = { title: "", isCompleted: false, description: ""}
  showTasksComplete: boolean = true;
  showTasksPending: boolean = true;
  noTasks: boolean = false;

  constructor(private authService: AuthService, private taskService: TaskService, private dialog: MatDialog) { }

  ngOnInit(): void {
    this.loadTasks();
    /*
    Notification.requestPermission().then((result) => {
        console.log(result)
      })

      let text = "Cuerpo de la notificación "
      let img = `${location.origin}/notificaciones.png`


      setTimeout(() => {
        new Notification("Nueva notificación ", {
          body: text,
          icon: img
        })
      }, 2000)
      */
  }

  loadTasks(): void {
    this.isLoading = true;
    this.taskService.show().subscribe({
      next: (data) => {
        this.tasks = data;
        this.isLoading = false;
        console.log("Tareas cargadas", this.tasks);
      },
      error: (err) => {
        this.errorMessage = "Error al cargar las tareas";
        this.isLoading = false;
        console.error(err);
      }
    })
  }

  openEditSideBar(task: ITask): void {
    this.selectedTask = { ...task }; //Clonamos para no dificar
    this.showSideBar = true;
  }

  closeSideBar(): void {
    this.showSideBar = false;
  }

  completeTask(taskId: number) {
    this.taskService.complete(taskId).subscribe({
      next: (res) => {
        console.log("Tarea completada con exito", res);
        this.loadTasks();
      },
      error: (err) => console.log("No se pudo completar la tares", err)
    })
  }

  createTask(form: NgForm) {
    if (form.valid) {
      this.taskService.create(this.taskData).subscribe({
        next: (res) => {
          console.log("Tarea agregada con exito", res);
          this.loadTasks();
          this.newTask = false;
          form.resetForm(); //Resetear el formulario
        },
        error: (err) => console.log("Error al agregar la tarea", err)
      });
    }
  }

  updateTask(form: NgForm): void {
    if (form.valid && this.selectedTask.taskId) {
      this.taskService.update(this.selectedTask, this.selectedTask.taskId).subscribe({
        next: () => {
          console.log("Tareas actualizada con exito");
          this.loadTasks();
          this.closeSideBar();
        },
        error: (err) => console.error("Error al actualizar tarea", err)
      })
    }
  }

  deleteTask(taskId: number): void {
    const dialogRef = this.dialog.open(ConfirmDeletetaskDialogComponent, {
      width: "350px",
      disableClose: true,
      enterAnimationDuration: "200ms",
      exitAnimationDuration: "150ms"
    });

    dialogRef.afterClosed().subscribe(res => {
      if (res) {
        this.taskService.Delete(taskId).subscribe({
          next: () => {
            console.log("Tarea eliminado con exito");
            this.tasks = this.tasks.filter(t => (t as any).taskId !== taskId);
          },
          error: (err) => {
            console.error("Error al eliminar la tarea", err);
          }
        })
      }
    })
  }

}
