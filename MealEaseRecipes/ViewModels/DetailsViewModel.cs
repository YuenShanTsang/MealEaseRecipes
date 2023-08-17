using CommunityToolkit.Mvvm.ComponentModel;
using MealEaseRecipes.Models;

namespace MealEaseRecipes.ViewModels
{
    public partial class DetailsViewModel : ObservableObject
    {
        private Recipe recipe;

        // Constructor to initialize the view model with the selected recipe
        public DetailsViewModel(Recipe selectedRecipe)
        {
            Title = "Recipes Details"; // Set the title of the details page
            recipe = selectedRecipe; // Assign the selected recipe to the private field
        }

        // Property to get the name of the recipe
        public string RecipeName => recipe?.RecipeName;

        // Property to get the image source of the recipe
        public ImageSource RecipeImage => recipe?.Image;

        // Property to get the type of meal (e.g., breakfast, lunch, dinner)
        public string MealType => recipe?.MealType;

        // Property to get the cooking time of the recipe
        public int CookingTime => recipe?.CookingTime ?? 0;

        // Property to get the required appliance or equipment for the recipe
        public string Equipment => recipe?.Equipment;

        // Property to get the list of ingredients for the recipe
        public string Ingredients => recipe?.Ingredients;

        // Property to get the step-by-step instructions for preparing the recipe
        public string Steps => recipe?.Steps;

        // Observable property to represent the title of the page
        [ObservableProperty]
        string title;
    }
}
