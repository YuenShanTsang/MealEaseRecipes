using MealEaseRecipes.Views;

namespace MealEaseRecipes;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        Routing.RegisterRoute(nameof(DetailsPage), typeof(DetailsPage));
        Routing.RegisterRoute(nameof(CreatePage), typeof(CreatePage));
        Routing.RegisterRoute(nameof(FavouritePage), typeof(FavouritePage));
    }
}
