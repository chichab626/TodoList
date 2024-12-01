using Plugin.Maui.Calendar.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TodoList.Data;
using TodoList.ViewModels;


namespace TodoList;

public class TodoListViewModel : BaseViewModel
{
    private readonly RestService _restService;
    private ObservableCollection<TodoViewModel> _todos;
    private ObservableCollection<Category> _categories;
    private EventCollection _events;
    private bool _isRefreshing;

    public TodoListViewModel(RestService restService)
    {
        _restService = restService;
        _todos = new ObservableCollection<TodoViewModel>();
        _events = new EventCollection();
        RefreshCommand = new Command(async () => await LoadTodosAsync());
        LoadTodosAsync();
    }

    public ObservableCollection<TodoViewModel> Todos
    {
        get => _todos;
        set
        {
            if (SetProperty(ref _todos, value))
            {
                // Whenever Todos changes, refresh the calendar events
                LoadTodosIntoCalendar();
            }
        }
    }

    public EventCollection Events
    {
        get => _events;
        set => SetProperty(ref _events, value);
    }

    public ObservableCollection<Category> Categories
    {
        get => _categories;
        set => SetProperty(ref _categories, value);
    }

    public bool IsRefreshing
    {
        get => _isRefreshing;
        set => SetProperty(ref _isRefreshing, value);
    }

    public ICommand RefreshCommand { get; }


    public async Task LoadCategoriesAsync()
    {
        Categories = new ObservableCollection<Category>(await _restService.GetCategoriesAsync());
    }

    public async Task LoadTodosAsync()
    {
        try
        {
            IsRefreshing = true;
            var todos = await _restService.GetTodosAsync();
            var categories = await _restService.GetCategoriesAsync();

            var todoViewModels = todos.Select(t => new TodoViewModel
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                IsCompleted = t.IsCompleted,
                CreatedAt = t.CreatedAt,
                Category = categories.FirstOrDefault(c => c.Id == t.CategoryId),
                DueDate = t.DueDate,
                DueTime = t.DueTime
            }).ToList();

            Todos = new ObservableCollection<TodoViewModel>(todoViewModels);
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    private void LoadTodosIntoCalendar()
    {
        // Create a new EventCollection each time to trigger UI updates
        var newEvents = new EventCollection();

        // Group todos by their due date
        var todosByDate = Todos
            .Where(t => t.DueDate != default)
            .GroupBy(t => t.DueDate.Date);

        foreach (var dateGroup in todosByDate)
        {
            newEvents[dateGroup.Key] = dateGroup

                .ToList();
        }

        Events = newEvents;
    }

    public async Task InitializeAsync()
    {
        await LoadTodosAsync();
        LoadTodosIntoCalendar();
    }
}
