using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoList.Api.Services
{
    public class TodoItemService : ITodoItemService
    {
        private readonly TodoContext _context;

        public TodoItemService(TodoContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TodoItem>> GetAllTodoItemsAsync()
        {
            return await _context.TodoItems.Where(x => !x.IsCompleted).ToListAsync();
        }

        public async Task<TodoItem> GetTodoItemAsync(Guid id)
        {
            return await _context.TodoItems.FindAsync(id);
        }

        public async Task<TodoItem> UpdateTodoItemAsync(Guid id, TodoItem newTodoItem)
        {
            _context.Entry(newTodoItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return newTodoItem;
        }

        public async Task<TodoItem> AddTodoItemAsync(TodoItem todoItem)
        {
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();
            return todoItem;
        }

        public async Task<bool> TodoItemIdExistsAsync(Guid id)
        {
            return await _context.TodoItems.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> TodoItemDescriptionExistsAsync(string description)
        {
            return await _context.TodoItems
                                .AnyAsync(x => x.Description.ToLowerInvariant() == description.ToLowerInvariant() && !x.IsCompleted);
        }
    }
}