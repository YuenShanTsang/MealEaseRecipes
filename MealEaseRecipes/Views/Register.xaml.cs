using MealEaseRecipes.ViewModels;

namespace MealEaseRecipes.Views;

public partial class Register : ContentPage
{
	public Register()
	{
		InitializeComponent();
        BindingContext = new RegisterViewModel(Navigation);

    }
}