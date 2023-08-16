using MealEase_Recipes.ViewModels;

namespace MealEase_Recipes.Views.Recipe;

public partial class EditRecipePage : ContentPage
{
	public EditRecipePage(Models.Recipe recipe)
	{
		InitializeComponent();
        BindingContext = new EditPageViewModel(Navigation, recipe);
    }
}