using Microsoft.Extensions.Logging;
using TodoList.Data;

namespace TodoList
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<RestService>();
            //builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<CalendarPage>();
            builder.Services.AddSingleton<CategoriesPage>();
            builder.Services.AddSingleton<TodoListPage>();
            builder.Services.AddSingleton<CategoryEditPage>();
            builder.Services.AddSingleton<TodoDetailPage>();
            return builder.Build();

        }
    }
}
