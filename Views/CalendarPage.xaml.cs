
using Plugin.Maui.Calendar.Models;
using System.Diagnostics;
using TodoList.Data;
namespace TodoList;

public partial class CalendarPage : ContentPage
{
    private readonly RestService _restService;
    private readonly TodoListViewModel _todoListViewModel;
    public EventCollection Events { get; set; }

    public CalendarPage(RestService restService)
    {
        InitializeComponent();
        _restService = restService;
        _todoListViewModel = new TodoListViewModel(_restService);
        _ = _todoListViewModel.InitializeAsync();
        BindingContext = _todoListViewModel;

        _restService.TodoItemUpdated += OnTodoItemUpdated;
    }

    private void OnTodoItemUpdated(int Id)
    {
        _ = _todoListViewModel.InitializeAsync();
    }
}