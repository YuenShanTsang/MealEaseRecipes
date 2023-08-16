using MealEaseRecipes.ViewModels;

namespace MealEaseRecipes.Views.Recipe;

public partial class EditRecipePage : ContentPage
{
	public EditRecipePage(Models.Recipe recipe)
	{
		InitializeComponent();
        BindingContext = new EditPageViewModel(Navigation, recipe);
    }
}