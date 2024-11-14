using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoList.Data
{
    public class TodoItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public int CategoryId { get; set; }
        public DateTime CreatedAt { get; set; }

        // Add new properties for Due Date and Due Time
        public DateTime DueDate { get; set; }
        public TimeSpan DueTime { get; set; }
    }

    public class Category
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public bool IsDefault { get; set; }
    }



public class DatabaseService
    {
        private readonly SQLiteAsyncConnection Database;
        private bool isInitialized = false;
        private readonly SemaphoreSlim initLock = new SemaphoreSlim(1, 1);

        public DatabaseService()
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "TodoList.db");
            Database = new SQLiteAsyncConnection(dbPath);
        }

        private async Task InitializeAsync()
        {
            if (!isInitialized)
            {
                await initLock.WaitAsync();
                try
                {
                    if (!isInitialized)
                    {
                        await Database.CreateTableAsync<TodoItem>();
                        await Database.CreateTableAsync<Category>();

                        var defaultCategory = await Database.Table<Category>()
                            .Where(c => c.IsDefault)
                            .FirstOrDefaultAsync();

                        if (defaultCategory == null)
                        {
                            await Database.InsertAsync(new Category
                            {
                                Name = "General",
                                Color = "#808080",
                                IsDefault = true
                            });
                        }
                        isInitialized = true;
                    }
                }
                finally
                {
                    initLock.Release();
                }
            }
        }

        // CRUD methods for Todo
        public async Task<List<TodoItem>> GetTodosAsync()
        {
            await InitializeAsync();
            return await Database.Table<TodoItem>().ToListAsync();
        }

        public async Task<TodoItem> GetTodoAsync(int id)
        {
            await InitializeAsync();
            return await Database.Table<TodoItem>().FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<int> SaveTodoAsync(TodoItem todo)
        {
            await InitializeAsync();
            if (todo.Id != 0)
            {
                return await Database.UpdateAsync(todo);
            }
            else
            {
                return await Database.InsertAsync(todo);
            }
        }

        public async Task<int> DeleteTodoAsync(int id)
        {
            await InitializeAsync();
            return await Database.DeleteAsync<TodoItem>(id);
        }

        // CRUD methods for Category
        public async Task<List<Category>> GetCategoriesAsync()
        {
            await InitializeAsync();
            return await Database.Table<Category>().ToListAsync();
        }

        public async Task<int> SaveCategoryAsync(Category category)
        {
            await InitializeAsync();
            if (category.Id != 0)
            {
                return await Database.UpdateAsync(category);
            }
            else
            {
                return await Database.InsertAsync(category);
            }
        }

        public async Task<int> DeleteCategoryAsync(int id)
        {
            await InitializeAsync();
            return await Database.DeleteAsync<Category>(id);
        }
    }

}
