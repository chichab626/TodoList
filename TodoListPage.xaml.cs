using System.Collections.ObjectModel;
using System.Windows.Input;
using TodoList.Data;

namespace TodoList;

public partial class TodoListPage : ContentPage
{
    private readonly DatabaseService _databaseService;
    private ObservableCollection<TodoViewModel> _todos;
    private ObservableCollection<Category> _categories;
    private bool _isRefreshing;

    public TodoListPage(DatabaseService databaseService)
    {
        InitializeComponent();
        _databaseService = databaseService;
        _todos = new ObservableCollection<TodoViewModel>();
        BindingContext = this;

        LoadTodosAsync();
    }

    public ObservableCollection<TodoViewModel> Todos
    {
        get => _todos;
        set
        {
            _todos = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Category> Categories
    {
        get => _categories;
        set
        {
            _categories = value;
            OnPropertyChanged();
        }
    }

    public bool IsRefreshing
    {
        get => _isRefreshing;
        set
        {
            _isRefreshing = value;
            OnPropertyChanged();
        }
    }

    public ICommand RefreshCommand => new Command(async () => await LoadTodosAsync());

    private async Task LoadCategoriesAsync()
    {
        Categories = new ObservableCollection<Category>(await _databaseService.GetCategoriesAsync());
    }

    private async Task LoadTodosAsync()
    {
        try
        {
            IsRefreshing = true;
            var todos = await _databaseService.GetTodosAsync();
            var categories = await _databaseService.GetCategoriesAsync();

            var todoViewModels = todos.Select(t => new TodoViewModel
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                IsCompleted = t.IsCompleted,
                CreatedAt = t.CreatedAt, // Keep CreatedAt in TodoViewModel
                Category = categories.FirstOrDefault(c => c.Id == t.CategoryId) // Set the Category object
            }).ToList();

            Todos = new ObservableCollection<TodoViewModel>(todoViewModels);
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    private async void OnAddTodoClicked(object sender, EventArgs e)
    {

        string title = await DisplayPromptAsync("New Todo", "Enter todo title");
        if (string.IsNullOrWhiteSpace(title))
            return;

        string description = await DisplayPromptAsync("New Todo", "Enter description (optional)");

        // Prompt user to select a category (you may want to implement a method for this)
        var categories = await _databaseService.GetCategoriesAsync();
        var selectedCategory = await DisplayActionSheet("Select a category", "Cancel", null,
            categories.Select(c => c.Name).ToArray());

        // Find the selected category by its name
        var category = categories.FirstOrDefault(c => c.Name == selectedCategory);

        if (category == null) return; // If user cancels or selects an invalid category

        var todo = new TodoItem
        {
            Title = title,
            Description = description,
            CategoryId = category.Id,
            CreatedAt = DateTime.Now // Set CreatedAt when creating a new TodoItem
        };

        await _databaseService.SaveTodoAsync(todo);
        await LoadTodosAsync();
    }

    private async void OnEditSwipeItemInvoked(object sender, EventArgs e)
    {
        TodoViewModel todo = null;

        if (sender is SwipeItem swipeItem)
        {
            todo = swipeItem.CommandParameter as TodoViewModel;
        }
        else if (sender is Button button)
        {
            todo = button.CommandParameter as TodoViewModel;
        }

        if (todo == null) return;

        // Prompt user for new title and description
        string title = await DisplayPromptAsync("Edit Todo", "Enter new title", initialValue: todo.Title);
        if (string.IsNullOrWhiteSpace(title))
            return;

        string description = await DisplayPromptAsync("Edit Todo", "Enter new description", initialValue: todo.Description);

        // Prompt user to select a category (you may want to implement a method for this)
        var categories = await _databaseService.GetCategoriesAsync();
        var selectedCategory = await DisplayActionSheet("Select a category", "Cancel", null,
            categories.Select(c => c.Name).ToArray());

        // Find the selected category by its name
        var category = categories.FirstOrDefault(c => c.Name == selectedCategory);

        if (category == null) return; // If user cancels or selects an invalid category

        // Create updated todo item with new values
        var updatedTodo = new TodoItem
        {
            Id = todo.Id,
            Title = title,
            Description = description,
            IsCompleted = todo.IsCompleted,
            CategoryId = category.Id, // Set the selected category's Id
            CreatedAt = todo.CreatedAt
        };

        // Save updated todo item to the database
        await _databaseService.SaveTodoAsync(updatedTodo);
        await LoadTodosAsync();
    }


    private async void OnDeleteSwipeItemInvoked(object sender, EventArgs e)
    {
        TodoViewModel todo = null;

        if (sender is SwipeItem swipeItem)
        {
            todo = swipeItem.CommandParameter as TodoViewModel;
        }
        else if (sender is Button button)
        {
            todo = button.CommandParameter as TodoViewModel;
        }

        if (todo == null) return;

        bool answer = await DisplayAlert("Delete Todo", "Are you sure you want to delete this todo?", "Yes", "No");
        if (!answer) return;

        await _databaseService.DeleteTodoAsync(todo.Id);
        await LoadTodosAsync();
    }

    private async void OnTodoCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox checkBox && checkBox.BindingContext is TodoViewModel todo)
        {

            todo.IsCompleted = e.Value; // Update the property

            var updatedTodo = new TodoItem
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                IsCompleted = e.Value,
                CategoryId = todo.Category.Id,
                CreatedAt = todo.CreatedAt // Keep CreatedAt unchanged
            };
            await _databaseService.SaveTodoAsync(updatedTodo);
            await LoadTodosAsync();

        }
    }
}
