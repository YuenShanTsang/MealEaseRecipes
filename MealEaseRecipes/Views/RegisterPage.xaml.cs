using MealEaseRecipes.ViewModels;

namespace MealEaseRecipes.Views;

public partial class RegisterPage : ContentPage
{
	public RegisterPage()
	{
		InitializeComponent();
        BindingContext = new RegisterViewModel(Navigation);
    }
}
