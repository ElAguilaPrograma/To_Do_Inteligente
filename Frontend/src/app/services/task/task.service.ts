import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { ITask } from "../../models/tasks.models/task.model";
import { Observable, tap } from "rxjs";

@Injectable({
  providedIn: "root"
})

export class TaskService {
  private baseUrl = "https://localhost:7035/api/Tasks";

  constructor(private http: HttpClient) { }
  //PARA CONTATENAR ES CON UN backticks ``
  create(taskInfo: ITask): Observable<any> {
    return this.http.post(`${this.baseUrl}/createtask`, taskInfo).pipe(
      tap((res: any) => console.log(res))
    )
  }

  update(taskInfoUpdate: ITask, taskId: number): Observable<any> {
    return this.http.put(`${this.baseUrl}/updatetask/${taskId}`, taskInfoUpdate).pipe(
      tap((res: any) => console.log(res))
    )
  }

  complete(taskId: number): Observable<any> {
    return this.http.patch(`${this.baseUrl}/completetask/${taskId}`, {});
  }

  show(): Observable<any> {
    return this.http.get(`${this.baseUrl}/showtasks`);
  }

  Delete(taskId: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/deletetask/${taskId}`);
  }

}
