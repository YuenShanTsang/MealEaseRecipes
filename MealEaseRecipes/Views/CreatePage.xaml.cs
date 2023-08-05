using MealEaseRecipes.ViewModels;

namespace MealEaseRecipes.Views;

public partial class CreatePage : ContentPage
{
	public CreatePage()
	{
		InitializeComponent();

        BindingContext = new CreateViewModel();
    }
}
