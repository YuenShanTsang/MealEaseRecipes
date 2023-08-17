using MealEaseRecipes.ViewModels;

namespace MealEaseRecipes.Views;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();

        BindingContext = new MainViewModel(Navigation);
     
    }
   
}
