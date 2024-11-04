using CommunityToolkit.Maui.Views;

namespace TodoList;

public partial class ColorPickerPopup : Popup
{
    private readonly List<Color> _predefinedColors;  // Remove the initializer here
    public Color SelectedColor { get; private set; }

    public ColorPickerPopup()
    {
        // Move the initialization to the constructor
        _predefinedColors = new List<Color> {
            Microsoft.Maui.Graphics.Colors.Red,
            Microsoft.Maui.Graphics.Colors.Blue,
            Microsoft.Maui.Graphics.Colors.Green,
            Microsoft.Maui.Graphics.Colors.Orange,
            Microsoft.Maui.Graphics.Colors.Purple,
            Microsoft.Maui.Graphics.Colors.Teal,
            Microsoft.Maui.Graphics.Colors.Pink,
            Microsoft.Maui.Graphics.Colors.Brown,
            Microsoft.Maui.Graphics.Colors.Indigo,
            Microsoft.Maui.Graphics.Colors.Gray
        };
        InitializeComponent();
        BindingContext = this;
    }

    public List<Color> Colors => _predefinedColors;

    private void OnColorSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Color selectedColor)
        {
            SelectedColor = selectedColor;
            Close(selectedColor);
        }
    }

    private void OnColorFrameTapped(object sender, EventArgs e)
    {
        if (sender is Frame frame && frame.BackgroundColor is Color tappedColor)
        {
            SelectedColor = tappedColor;
            Close(tappedColor);
        }
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        Close();
    }
}
