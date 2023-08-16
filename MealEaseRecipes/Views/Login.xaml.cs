using MealEaseRecipes.ViewModels;

namespace MealEaseRecipes.Views;

public partial class Login : ContentPage
{
 
    public Login()
	{
		InitializeComponent();
        BindingContext = new LoginViewModel(Navigation);
    }
   
}