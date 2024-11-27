using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace TodoList.Data
{
    public class RestService
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _serializerOptions;

        public static string BaseAddress =
    DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5087" : "http://localhost:5087";
        public static string TodoItemsUrl = $"{BaseAddress}/api/TodoItems/";
        public static string CategoriesUrl = $"{BaseAddress}/api/Categories/";

        public List<TodoItem> TodoItems { get; private set; }
        public List<Category> Categories { get; private set; }

        public RestService()
        {
            _client = new HttpClient();
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        // Fetch all TodoItems
        public async Task<List<TodoItem>> GetTodosAsync()
        {
            var response = await _client.GetAsync(TodoItemsUrl);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                TodoItems = JsonSerializer.Deserialize<List<TodoItem>>(json, _serializerOptions);
            }
            return TodoItems;
        }

        // Fetch a specific TodoItem by Id
        public async Task<TodoItem> GetTodoAsync(int id)
        {
            var response = await _client.GetAsync($"https://api.example.com/todos/{id}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TodoItem>(json, _serializerOptions);
            }
            return null;
        }

        // Create or update a TodoItem
        public async Task<int> SaveTodoAsync(TodoItem todo)
        {
            HttpResponseMessage response;
            if (todo.Id == 0)  // Create new Todo
            {
                response = await _client.PostAsJsonAsync("https://api.example.com/todos", todo);
            }
            else  // Update existing Todo
            {
                response = await _client.PutAsJsonAsync($"https://api.example.com/todos/{todo.Id}", todo);
            }
            return response.IsSuccessStatusCode ? 1 : 0;
        }

        // Delete a TodoItem
        public async Task<int> DeleteTodoAsync(int id)
        {
            var response = await _client.DeleteAsync($"https://api.example.com/todos/{id}");
            return response.IsSuccessStatusCode ? 1 : 0;
        }

        // Fetch all Categories
        public async Task<List<Category>> GetCategoriesAsync()
        {
            var response = await _client.GetAsync(CategoriesUrl);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Categories = JsonSerializer.Deserialize<List<Category>>(json, _serializerOptions);
            }
            return Categories;
        }

        // Save a new or updated Category
        public async Task<int> SaveCategoryAsync(Category category)
        {
            HttpResponseMessage response;
            if (category.Id == 0)  // Create new Category
            {
                response = await _client.PostAsJsonAsync("https://api.example.com/categories", category);
            }
            else  // Update existing Category
            {
                response = await _client.PutAsJsonAsync($"https://api.example.com/categories/{category.Id}", category);
            }
            return response.IsSuccessStatusCode ? 1 : 0;
        }

        // Delete a Category
        public async Task<int> DeleteCategoryAsync(int id)
        {
            var response = await _client.DeleteAsync($"https://api.example.com/categories/{id}");
            return response.IsSuccessStatusCode ? 1 : 0;
        }
    }
}
