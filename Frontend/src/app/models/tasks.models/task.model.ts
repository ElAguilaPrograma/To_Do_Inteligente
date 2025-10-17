export interface ITask {
  taskId?: number;
  title: string;
  isCompleted: boolean;
  description: string;
  reminderDate?: Date;
}
//ASP.NET devuelve las propiedades como camelCase osea que IsCompleted debe escribirse isCompleted en Angular
