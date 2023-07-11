using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MealEaseRecipes.Models;
using MealEaseRecipes.Views;

namespace MealEaseRecipes.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public ObservableCollection<Recipe> Recipes { get; } = new ObservableCollection<Recipe>();


        public MainViewModel()
        {
            Title = "MealEase Recipes";

            // Add example recipes to the collection
            Recipes.Add(new Recipe
            {
                RecipeName = "Recipe 1",
                RecipeImage = "https://www.eatingwell.com/thmb/P3WwF1tZ1iFaOvKQDnJthqT4kdY=/1500x0/filters:no_upscale():max_bytes(150000):strip_icc()/Quick-Cooking-Oats-0175-1-b7bb0c5ed67a43569c1d7bae7a75ae7a.jpg",
                MealType = "Breakfast",
                CookTime = 10,
                Appliance = "Stove",
                Ingredients = "Ingredient 1, Ingredient 2, Ingredient 3",
                Steps = "Step 1, Step 2, Step 3"
            });

            Recipes.Add(new Recipe
            {
                RecipeName = "Recipe 2",
                RecipeImage = "https://www.thespruceeats.com/thmb/cQD_JvUOD_OzgrQkF0cUiqobkyE=/1500x0/filters:no_upscale():max_bytes(150000):strip_icc()/vegetarian-bean-and-rice-burrito-recipe-3378550-9_preview-5b2417e1ff1b780037a58cda.jpeg",
                MealType = "Lunch",
                CookTime = 20,
                Appliance = "Microwaves",
                Ingredients = "Ingredient 4, Ingredient 5, Ingredient 6",
                Steps = "Step 4, Step 5, Step 6"
            });

            Recipes.Add(new Recipe
            {
                RecipeName = "Recipe 3",
                RecipeImage = "https://www.acouplecooks.com/wp-content/uploads/2019/03/Mushroom-Pasta-007.jpg",
                MealType = "Dinner",
                CookTime = 30,
                Appliance = "Oven",
                Ingredients = "Ingredient 7, Ingredient 8, Ingredient 9",
                Steps = "Step 7, Step 8, Step 9"
            });

        }

        [ObservableProperty]
        string title;


        [RelayCommand]
        public async Task GoToDetailsPage(Recipe selectedRecipe)
        {
            var detailsViewModel = new DetailsViewModel(selectedRecipe);
            var detailsPage = new DetailsPage(detailsViewModel);
            await Shell.Current.Navigation.PushAsync(detailsPage);
        }

    }
}

