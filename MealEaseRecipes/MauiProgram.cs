using MealEaseRecipes.Models;
using MealEaseRecipes.ViewModels;
using MealEaseRecipes.Views;
using Microsoft.Extensions.Logging;

namespace MealEaseRecipes;

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

        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<DetailsViewModel>();
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<DetailsPage>();
        builder.Services.AddTransient<Recipe>();


#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}

