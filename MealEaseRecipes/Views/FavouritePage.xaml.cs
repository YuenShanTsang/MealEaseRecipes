using MealEase_Recipes.ViewModels;

namespace MealEase_Recipes.Views;

public partial class FavouritePage : ContentPage
{
	public FavouritePage()
	{
		InitializeComponent();
        BindingContext = new FavouriteViewModel(Navigation);
    }

}
