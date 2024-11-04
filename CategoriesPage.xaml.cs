using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TodoList.Data;

namespace TodoList;

public partial class CategoriesPage : ContentPage
{
    private readonly DatabaseService _databaseService;
    private ObservableCollection<Category> _categories;
    private bool _isRefreshing;

    public CategoriesPage(DatabaseService databaseService)
    {
        InitializeComponent();
        _databaseService = databaseService;
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
            var categories = await _databaseService.GetCategoriesAsync();

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
        var page = new NavigationPage(new CategoryEditPage(_databaseService));
        page.Disappearing += async (s, args) => await LoadCategoriesAsync();
        await Navigation.PushModalAsync(page);
    }

    private async void OnAddCategoryClicked_old(object sender, EventArgs e)
    {
        string name = await DisplayPromptAsync("New Category", "Enter category name");
        if (string.IsNullOrWhiteSpace(name))
            return;

        var colorPicker = new ColorPickerPopup();
        var result = await this.ShowPopupAsync(colorPicker);

        if (result is not Color selectedColor)
            return;

        var category = new Category
        {
            Name = name,
            Color = selectedColor.ToHex(),
            IsDefault = false
        };

        try
        {
            await _databaseService.SaveCategoryAsync(category);
            await LoadCategoriesAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async Task OnEditCategoryAsync_old(Category category)
    {
        if (category == null) return;

        string name = await DisplayPromptAsync("Edit Category", "Enter new name", initialValue: category.Name);
        if (string.IsNullOrWhiteSpace(name))
            return;

        var colorPicker = new ColorPickerPopup();
        var result = await this.ShowPopupAsync(colorPicker);

        if (result is not Color selectedColor)
            return;

        category.Name = name;
        category.Color = selectedColor.ToHex();

        try
        {
            await _databaseService.SaveCategoryAsync(category);
            await LoadCategoriesAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async Task OnEditCategoryAsync(Category category)
    {
        if (category == null) return;

        var page = new NavigationPage(new CategoryEditPage(_databaseService, category));
        page.Disappearing += async (s, args) => await LoadCategoriesAsync();
        await Navigation.PushModalAsync(page);
    }

    private async Task OnDeleteCategoryAsync(Category category)
    {
        if (category == null) return;

        bool answer = await DisplayAlert("Delete Category",
            "Are you sure you want to delete this category? All todos in this category will be moved to General category.",
            "Yes", "No");

        if (!answer) return;

        try
        {
            await _databaseService.DeleteCategoryAsync(category.Id);
            await LoadCategoriesAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

}
