using MealEaseRecipes.ViewModels;

namespace MealEaseRecipes.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();

        BindingContext = new LoginViewModel(Navigation);
    }
}
