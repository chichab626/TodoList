using System.Collections.ObjectModel;
using System.Windows.Input;
using TodoList.Data;

namespace TodoList;

public partial class CategoriesPage : ContentPage
{
    private readonly RestService _restService;
    private ObservableCollection<Category> _categories;
    private bool _isRefreshing;

    public CategoriesPage(RestService restService)
    {
        InitializeComponent();
        _restService = restService;
        _categories = new ObservableCollection<Category>();
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadCategoriesAsync();
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

    public ICommand DeleteCommand  => new Command<Category>(async (category) => await OnDeleteCategoryAsync(category));
    public ICommand EditCommand => new Command<Category>(async (category) => await OnEditCategoryAsync(category));
    public ICommand RefreshCommand => new Command(async () => await LoadCategoriesAsync());

    private async Task LoadCategoriesAsync()
    {
        try
        {
            IsRefreshing = true;
            var categories = await _restService.GetCategoriesAsync();

            Categories.Clear();
            foreach (var category in categories)
            {
                Categories.Add(category);
            }
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    private async void OnAddCategoryClicked(object sender, EventArgs e)
    {
        var page = new NavigationPage(new CategoryEditPage(_restService));
        page.Disappearing += async (s, args) => await LoadCategoriesAsync();
        await Navigation.PushModalAsync(page);
    }

    

    private async Task OnEditCategoryAsync(Category category)
    {
        if (category == null) return;

        var page = new NavigationPage(new CategoryEditPage(_restService, category));
        page.Disappearing += async (s, args) => await LoadCategoriesAsync();
        await Navigation.PushModalAsync(page);
    }

    private async Task OnDeleteCategoryAsync(Category category)
    {
        if (category == null) return;

        bool answer = await DisplayAlert("Delete Category",
            "Are you sure you want to delete this category?",
            "Yes", "No");

        if (!answer) return;

        try
        {
            var result = await _restService.DeleteCategoryAsync(category.Id);
            if (result == 0)
            {
                throw new Exception("Cannot delete this category. Make sure there are no Todo items in this category.");
            }
            await LoadCategoriesAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

}
