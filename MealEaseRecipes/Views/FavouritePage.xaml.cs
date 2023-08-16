namespace MealEaseRecipes.Views;

public partial class FavouritePage : ContentPage
{
    public FavouritePage()
    {
        InitializeComponent();
        BindingContext = new FavouriteViewModel(Navigation);
    }

}