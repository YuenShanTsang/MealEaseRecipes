using MealEaseRecipes.ViewModels;

namespace MealEaseRecipes.Views;

public partial class FavouritePage : ContentPage
{
    public FavouritePage()
    {
        InitializeComponent();

        BindingContext = new MainViewModel(Navigation);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Refresh the data when the page becomes visible
        ((MainViewModel)BindingContext).FetchFavoriteRecipes();
    }

}