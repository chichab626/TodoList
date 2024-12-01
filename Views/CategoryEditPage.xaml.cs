using System.Windows.Input;
using TodoList.Data;

namespace TodoList;

public partial class CategoryEditPage : ContentPage
{
    private readonly RestService _restService;
    private readonly Category _category;
    private readonly bool _isEdit;
    private string _categoryName;
    private Color _selectedColor;

    public CategoryEditPage(RestService restService, Category category = null)
    {
        InitializeComponent();
        _restService = restService;
        _category = category ?? new Category { IsDefault = false };
        _isEdit = category != null;

        CategoryName = _category.Name;
        if (!string.IsNullOrEmpty(_category.Color))
        {
            SelectedColor = Color.FromArgb(_category.Color);
        }

        BindingContext = this;
    }

    public string PageTitle => _isEdit ? "Edit Category" : "New Category";

    public string CategoryName
    {
        get => _categoryName;
        set
        {
            _categoryName = value;
            OnPropertyChanged();
        }
    }

    public Color SelectedColor
    {
        get => _selectedColor;
        set
        {
            _selectedColor = value;
            OnPropertyChanged();
        }
    }

    public List<Color> Colors => new()
    {
        Microsoft.Maui.Graphics.Colors.Red,
        Microsoft.Maui.Graphics.Colors.Blue,
        Microsoft.Maui.Graphics.Colors.Green,
        Microsoft.Maui.Graphics.Colors.Orange,
        Microsoft.Maui.Graphics.Colors.Purple,
        Microsoft.Maui.Graphics.Colors.Teal,
        Microsoft.Maui.Graphics.Colors.Pink,
        Microsoft.Maui.Graphics.Colors.Brown,
        Microsoft.Maui.Graphics.Colors.Indigo,
        Microsoft.Maui.Graphics.Colors.Gray,
        Microsoft.Maui.Graphics.Colors.Yellow,
        Microsoft.Maui.Graphics.Colors.Cyan,
        Microsoft.Maui.Graphics.Colors.Magenta,
        Microsoft.Maui.Graphics.Colors.Lime,
        Microsoft.Maui.Graphics.Colors.Violet,
        Microsoft.Maui.Graphics.Colors.Coral,
        Microsoft.Maui.Graphics.Colors.Salmon,
        Microsoft.Maui.Graphics.Colors.Lavender,
        Microsoft.Maui.Graphics.Colors.Olive,
        Microsoft.Maui.Graphics.Colors.Navy

    };

    public ICommand SelectColorCommand => new Command<Color>(color => SelectedColor = color);

    public ICommand SaveCommand => new Command(async () => await SaveCategoryAsync());

    public ICommand CancelCommand => new Command(async () => await Navigation.PopModalAsync());

    private async Task SaveCategoryAsync()
    {
        if (string.IsNullOrWhiteSpace(CategoryName))
        {
            await DisplayAlert("Error", "Please enter a category name", "OK");
            return;
        }

        if (SelectedColor == null)
        {
            await DisplayAlert("Error", "Please select a color", "OK");
            return;
        }

        try
        {
            _category.Name = CategoryName;
            _category.Color = SelectedColor.ToHex();

            await _restService.SaveCategoryAsync(_category);
            await Navigation.PopModalAsync();

        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}