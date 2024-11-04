using System.Collections.ObjectModel;
using System.Windows.Input;
using TodoList.Data;

namespace TodoList;

public partial class TodoDetailPage : ContentPage
{
    private readonly DatabaseService _databaseService;
    private readonly Category _category;
    public TodoViewModel Todo { get; set; }
    public ObservableCollection<Category> Categories { get; }

    public string PageTitle { get; set; }

    private Category _selectedCategory;
    public Category SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            _selectedCategory = value;
            OnPropertyChanged();
        }
    }

    public TodoDetailPage(DatabaseService databaseService, TodoViewModel todo = null)
	{
        _databaseService = databaseService;
        Todo = todo ?? new TodoViewModel();
        PageTitle = Todo.Id == 0 ? "Add Todo" : "Edit Todo";
        Categories = new ObservableCollection<Category>();
        InitializeComponent();
        BindingContext = this;

        LoadCategoriesAsync();

    }

    public ICommand CancelCommand => new Command(async () => await Navigation.PopModalAsync());
    public ICommand SaveCommand => new Command(async () => await SaveTodoAsync());

    private async Task SaveTodoAsync()
    {
        // Validate title input
        if (string.IsNullOrWhiteSpace(Todo.Title))
        {
            await DisplayAlert("Error", "Please enter the title", "OK");
            return;
        }

        // Validate selected category
        if (SelectedCategory == null)
        {
            await DisplayAlert("Error", "Please select a category", "OK");
            return;
        }

        try
        {
            // Create a new TodoItem with the provided details
            TodoItem todoItem;

            if (Todo.Id == 0)
            {
                // Create a new todo item
                todoItem = new TodoItem
                {
                    Title = Todo.Title,
                    Description = Todo.Description,
                    IsCompleted = Todo.IsCompleted,
                    CategoryId = SelectedCategory.Id, // Use the selected category
                    CreatedAt = DateTime.Now // Set CreatedAt only for new todos
                };
            }
            else
            {
                // Update the existing todo item
                todoItem = new TodoItem
                {
                    Id = Todo.Id,
                    Title = Todo.Title,
                    Description = Todo.Description,
                    IsCompleted = Todo.IsCompleted,
                    CategoryId = SelectedCategory.Id, // Use the selected category
                    CreatedAt = Todo.CreatedAt // Keep the original CreatedAt
                };
            }

            // Save the todo item to the database
            await _databaseService.SaveTodoAsync(todoItem);

            // Navigate back to the TodoListPage
            await Navigation.PopModalAsync();
        }
        catch (Exception ex)
        {
            // Handle any errors that occur during the save operation
            await DisplayAlert("Error", $"An error occurred while saving the todo: {ex.Message}", "OK");
        }
    }


    private async Task LoadCategoriesAsync()
    {
        try
        {
            // Retrieve categories from the database
            var categoriesFromDb = await _databaseService.GetCategoriesAsync();

            // Clear existing categories (if any) and load the new data
            Categories.Clear();
            foreach (var category in categoriesFromDb)
            {
                Categories.Add(category);
            }

            SelectedCategory = Categories.FirstOrDefault(c => c.Id == Todo.Category.Id);
        }
        catch (Exception ex)
        {
            // Handle exceptions (e.g., log error or show a message to the user)
            Console.WriteLine($"Error loading categories: {ex.Message}");
        }
    }

}
