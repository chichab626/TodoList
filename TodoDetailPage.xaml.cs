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

        // Combine DueDate and DueTime into a single DateTime object
        DateTime selectedDueDateTime = Todo.DueDate.Date.Add(Todo.DueTime);

        // Check if the selected due date and time is in the past
        if (selectedDueDateTime < DateTime.Now)
        {
            await DisplayAlert("Error", "The due date and time cannot be in the past", "OK");
            return;
        }

        try
        {
            // Create a new TodoItem with the provided details
            TodoItem todoItem = new TodoItem
            {
                Id = Todo.Id,
                Title = Todo.Title,
                Description = Todo.Description,
                IsCompleted = Todo.IsCompleted,
                CategoryId = SelectedCategory.Id,

                // Update DueDate and DueTime
                DueDate = Todo.DueDate,
                DueTime = Todo.DueTime
            };

            if (Todo.Id == 0)
            {
                todoItem.CreatedAt = DateTime.Now;
            }
            else
            {
                todoItem.CreatedAt = Todo.CreatedAt;
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
