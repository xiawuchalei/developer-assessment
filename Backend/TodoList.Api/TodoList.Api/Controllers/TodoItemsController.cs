using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using TodoList.Api.Services;


namespace TodoList.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly ITodoItemService _todoItemService;

        private readonly ILogger<TodoItemsController> _logger;

        public TodoItemsController(ITodoItemService todoItemService, ILogger<TodoItemsController> logger)
        {
            _todoItemService = todoItemService;
            _logger = logger;
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<IActionResult> GetTodoItems()
        {
            try
            {
                var results = await _todoItemService.GetAllTodoItemsAsync();
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting todo items.");
                return StatusCode(500, "An error occurred while retrieving data.");
            }
        }

        // GET: api/TodoItems/...
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodoItem(Guid id)
        {
            try
            {
                var result = await _todoItemService.GetTodoItemAsync(id);
                if (result == null)
                {
                    return NotFound($"Todo item with ID {id} not found.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting todo item with ID {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the todo item.");
            }
        }

        // PUT: api/TodoItems/... 
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(Guid id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest("ID mismatch in the request.");
            }

            var existingTodoItem = await _todoItemService.GetTodoItemAsync(id);
            if (existingTodoItem != null)
            {
                try
                {
                    existingTodoItem.Description = todoItem.Description;
                    existingTodoItem.IsCompleted = todoItem.IsCompleted;
                    await _todoItemService.UpdateTodoItemAsync(id, existingTodoItem);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex, "Error updating todo item with ID {Id}", id);
                    return StatusCode(500, "An error occurred while updating the todo item.");
                }
            }
            else 
            {
                return NotFound($"Todo item with ID {id} not found.");
            }

            return NoContent();
        } 

        // POST: api/TodoItems 
        [HttpPost]
        public async Task<IActionResult> PostTodoItem(TodoItem todoItem)
        {
            if (string.IsNullOrEmpty(todoItem?.Description))
            {
                return BadRequest("Description is required");
            }
            else if (await TodoItemDescriptionExists(todoItem.Description))
            {
                return BadRequest("Description already exists");
            } 

            try
            {
                var newItem = await _todoItemService.AddTodoItemAsync(todoItem);
                return CreatedAtAction(nameof(GetTodoItem), new { id = newItem.Id }, newItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error posting new todo item.");
                return StatusCode(500, "An error occurred while adding the new todo item.");
            }
        } 

        private async Task<bool> TodoItemIdExists(Guid id)
        {
            return await _todoItemService.TodoItemIdExistsAsync(id);
        }

        private async Task<bool> TodoItemDescriptionExists(string description)
        {
            return await _todoItemService.TodoItemDescriptionExistsAsync(description);
        }
    }
}
