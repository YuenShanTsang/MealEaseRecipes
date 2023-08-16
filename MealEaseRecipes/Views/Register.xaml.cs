using MealEase_Recipes.ViewModels;

namespace MealEase_Recipes.Views;

public partial class Register : ContentPage
{
	public Register()
	{
		InitializeComponent();
        BindingContext = new RegisterViewModel(Navigation);

    }
}