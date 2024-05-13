import { Component, OnInit } from '@angular/core';
import { TodoItemService } from '../../services/todo-item.service';
import { TodoItem } from '../../models/todo-item.model'; 

@Component({
  selector: 'app-todo-item',
  templateUrl: './todo-item.component.html',
  styleUrls: ['./todo-item.component.css']
})
export class TodoItemComponent implements OnInit {
  items: TodoItem[] = [];
  description: string = '';

  constructor(private todoItemService: TodoItemService) {}

  ngOnInit(): void {
    this.getItems();
  }

  getItems(): void {
    this.todoItemService.getItems().subscribe(
      items => {
        this.items = items;
        this.items = this.items.sort((a, b) => a.description.localeCompare(b.description)); 
      },
      error => {
        console.error('Error fetching items', error);
        alert(`Error: ${error.error || 'Unknown error'}`);
      }
    );
  }

  handleAdd(): void {
    if (!this.description.trim()) {
      alert('Description cannot be empty');
      return;
    }
    const newItem = { description: this.description, isCompleted: false };
    this.todoItemService.addItem(newItem).subscribe(
      item => {
        this.items.push(item);
        this.items = this.items.sort((a, b) => a.description.localeCompare(b.description));
        this.description = '';
      },
      error => {
        alert(`Error: ${error.error || 'Unknown error'}`);
        console.error('Error adding item', error)
      }
    );
  }

  handleClear(): void {
    this.description = '';
  }

  handleMarkAsComplete(item: TodoItem): void {
    if (!item.isCompleted) {
      const updatedItem = { ...item, isCompleted: true };
      this.todoItemService.updateItem(item.id, updatedItem).subscribe(
        () => item.isCompleted = true,
        error => {
          alert(`Error: ${error.error || 'Unknown error'}`);
          console.error('Error completing item', error);
        }
      );
    }
    else {
      alert('This item is already marked as completed.');
      return;
    }
  }
}
