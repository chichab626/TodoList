using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using TodoList.Data;

namespace TodoList;

public partial class TodoListPage : ContentPage
{
    private readonly TodoListViewModel _viewModel;
    private readonly RestService _restService;

    public TodoListPage(RestService restService)
    {
        _restService = restService;
        InitializeComponent();
        _viewModel = new TodoListViewModel(restService);
        BindingContext = _viewModel;

        _restService.CategoryUpdated += OnCategoryUpdated;
    }

    private void OnCategoryUpdated(Category updatedCategory)
    {
        Debug.WriteLine("Subscribed again");
        foreach (var todo in _viewModel.Todos)
        {
            if (todo.Category.Id == updatedCategory.Id)
            {
                todo.Category = updatedCategory;
            }
        }
    }

    private async void OnAddTodoClicked(object sender, EventArgs e)
    {
        var page = new NavigationPage(new TodoDetailPage(_restService));
        page.Disappearing += async (s, args) => await _viewModel.LoadTodosAsync();
        await Navigation.PushModalAsync(page);
    }

    private async void OnEditActionInvoked(object sender, EventArgs e)
    {
        TodoViewModel todo = null;

        // Check if the sender is a swipe item or a button
        if (sender is SwipeItem swipeItem && swipeItem.CommandParameter is TodoViewModel swipeTodo)
        {
            todo = swipeTodo;  // Assign the todo item from the swipe
        }
        else if (sender is Button button && button.CommandParameter is TodoViewModel buttonTodo)
        {
            todo = buttonTodo;  // Assign the todo item from the button click
        }

        // If we found a todo item, navigate to the detail page
        if (todo != null)
        {
            var page = new NavigationPage(new TodoDetailPage(_restService, todo));
            page.Disappearing += async (s, args) => await _viewModel.LoadTodosAsync();
            await Navigation.PushModalAsync(page);
        }
    }

    private async void OnDeleteActionInvoked(object sender, EventArgs e)
    {
        TodoViewModel todo = null;

        // Check if the sender is a swipe item or a button
        if (sender is SwipeItem swipeItem && swipeItem.CommandParameter is TodoViewModel swipeTodo)
        {
            todo = swipeTodo;  // Assign the todo item from the swipe
        }
        else if (sender is Button button && button.CommandParameter is TodoViewModel buttonTodo)
        {
            todo = buttonTodo;  // Assign the todo item from the button click
        }

        // If a todo item is found, show the delete confirmation
        if (todo != null)
        {
            bool answer = await DisplayAlert("Delete Todo", "Are you sure you want to delete this todo?", "Yes", "No");
            if (answer)
            {
                await _restService.DeleteTodoAsync(todo.Id);
                await _viewModel.LoadTodosAsync();
            }
        }
    }


    private async void OnTodoCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox checkBox && checkBox.BindingContext is TodoViewModel todo)
        {
            todo.IsCompleted = e.Value;
            var updatedTodo = new TodoItem
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                IsCompleted = e.Value,
                CategoryId = todo.Category.Id,
                CreatedAt = todo.CreatedAt,
                DueTime = todo.DueTime,
                DueDate = todo.DueDate
            };

            await _restService.SaveTodoAsync(updatedTodo);
        }
    }
}
