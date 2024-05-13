using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoList.Api.Services
{
    public interface ITodoItemService
    {
        Task<IEnumerable<TodoItem>> GetAllTodoItemsAsync();
        Task<TodoItem> GetTodoItemAsync(Guid id);
        Task<TodoItem> UpdateTodoItemAsync(Guid id, TodoItem todoItem);
        Task<TodoItem> AddTodoItemAsync(TodoItem todoItem);
        Task<bool> TodoItemIdExistsAsync(Guid id);
        Task<bool> TodoItemDescriptionExistsAsync(string description);
    }

}
