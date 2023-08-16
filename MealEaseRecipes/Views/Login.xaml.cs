using MealEase_Recipes.ViewModels;

namespace MealEase_Recipes.Views;

public partial class Login : ContentPage
{
 
    public Login()
	{
		InitializeComponent();
        BindingContext = new LoginViewModel(Navigation);
    }
   
}