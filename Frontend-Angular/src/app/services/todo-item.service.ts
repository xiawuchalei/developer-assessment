import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TodoItem } from '../models/todo-item.model'; 

@Injectable({
  providedIn: 'root'
})
export class TodoItemService {
  private apiUrl = 'https://localhost:5001/api/TodoItems';

  constructor(private httpClient: HttpClient) { }

  // Get a list of all the items
  getItems(): Observable<TodoItem[]> {
    return this.httpClient.get<TodoItem[]>(this.apiUrl);
  }

  // Create a new todo item
  addItem(item: TodoItem): Observable<TodoItem> {
    return this.httpClient.post<TodoItem>(this.apiUrl, item);
  }

  // Mark the item as completed
  updateItem(id: number, item: TodoItem): Observable<any> {
    return this.httpClient.put(`${this.apiUrl}/${id}`, item);
  }
}
