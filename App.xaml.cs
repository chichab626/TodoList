using TodoList.Data;

namespace TodoList
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TodoList.db");

            MainPage = new AppShell();
        }

    }
}
