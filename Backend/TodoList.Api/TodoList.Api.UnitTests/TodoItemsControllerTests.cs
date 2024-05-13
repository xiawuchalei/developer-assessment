using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using TodoList.Api.Controllers;
using TodoList.Api.Services;

namespace TodoList.Api.UnitTests
{
    public class TodoItemsControllerTests: IDisposable
    {
        private readonly Mock<ILogger<TodoItemsController>> _mockLogger;
        private readonly ITodoItemService _todoItemService;
        private readonly TodoContext _context;
        private readonly TodoItemsController _controller;

        public TodoItemsControllerTests()
        {
            _mockLogger = new Mock<ILogger<TodoItemsController>>();
            var options = new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase(databaseName: "TodoListTestDb")
                .Options;
            _context = new TodoContext(options);
            _todoItemService = new TodoItemService(_context);
            _controller = new TodoItemsController(_todoItemService, _mockLogger.Object);

            // Seed the database
            _context.TodoItems.AddRange(new List<TodoItem>
            {
                new TodoItem { Id = Guid.NewGuid(), Description = "Task 1", IsCompleted = false },
                new TodoItem { Id = Guid.NewGuid(), Description = "Task 2", IsCompleted = false }
            });
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted(); 
            _context.Dispose();
        }

        [Fact]
        public async Task GetTodoItems_ReturnsOkResult_WithTodoItems()
        {
            var result = await _controller.GetTodoItems();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var items = Assert.IsType<List<TodoItem>>(okResult.Value);
            Assert.Equal(2, items.Count);
        }

        [Fact]
        public async Task GetTodoItem_ReturnsNotFound_ForNonexistentItem()
        {
            var result = await _controller.GetTodoItem(Guid.NewGuid());
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task PutTodoItem_ReturnsBadRequest_WhenIdsDoNotMatch()
        {
            var todoItem = new TodoItem { Id = Guid.NewGuid(), Description = "Mismatch ID Test", IsCompleted = false };
            var result = await _controller.PutTodoItem(Guid.NewGuid(), todoItem);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task PutTodoItem_UpdatesItem_WhenIdsMatch()
        {
            var item = _context.TodoItems.First();
            var updatedItem = new TodoItem { Id = item.Id, Description = "Updated Task", IsCompleted = true };
            var result = await _controller.PutTodoItem(item.Id, updatedItem);
            var updatedEntry = _context.TodoItems.Find(item.Id);

            Assert.IsType<NoContentResult>(result);
            Assert.Equal("Updated Task", updatedEntry.Description);
            Assert.True(updatedEntry.IsCompleted);
        }

        [Fact]
        public async Task PostTodoItem_ReturnsCreatedAtAction_WithValidData()
        {
            var newItem = new TodoItem { Description = "New Task", IsCompleted = false };
            var result = await _controller.PostTodoItem(newItem);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var item = Assert.IsType<TodoItem>(createdAtActionResult.Value);
            Assert.Equal("New Task", item.Description);
            Assert.False(item.IsCompleted);
        }
    }
}
