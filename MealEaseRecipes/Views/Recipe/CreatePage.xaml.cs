using MealEase_Recipes.ViewModels;

namespace MealEase_Recipes.Views;

public partial class CreatePage : ContentPage
{
	public CreatePage()
	{
		InitializeComponent();

        BindingContext = new CreateViewModel();
    }

}
