using CommunityToolkit.Mvvm.ComponentModel;
using MealEaseRecipes.Models;


namespace MealEaseRecipes.ViewModels
{
    public partial class DetailsViewModel : ObservableObject
    {
        private Recipe recipe;

        public DetailsViewModel(Recipe selectedRecipe)
        {
            Title = "Recipes Details";
            recipe = selectedRecipe;
        }

        public string RecipeName => recipe?.RecipeName;
        public ImageSource RecipeImage => recipe?.RecipeImage;
        public string MealType => recipe?.MealType;
        public int CookingTime => recipe?.CookingTime ?? 0;
        public string Appliance => recipe?.Equipment;
        public string Ingredients => recipe?.Ingredients;
        public string Steps => recipe?.Steps;

        [ObservableProperty]
        string title;

    }
}

