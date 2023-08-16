using MealEase_Recipes.ViewModels;

namespace MealEase_Recipes.Views;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();

        BindingContext = new MainViewModel(Navigation);
    }
}
