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
    private bool _isRefreshing;

    public TodoListViewModel(RestService restService)
    {
        _restService = restService;
        _todos = new ObservableCollection<TodoViewModel>();
        RefreshCommand = new Command(async () => await LoadTodosAsync());
        LoadTodosAsync();
    }

    public ObservableCollection<TodoViewModel> Todos
    {
        get => _todos;
        set => SetProperty(ref _todos, value);
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
}
