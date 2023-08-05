using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MealEaseRecipes.Models;
using MealEaseRecipes.Views;
using System.Diagnostics;

namespace MealEaseRecipes.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {


        public ObservableCollection<Recipe> Recipes { get; } = new ObservableCollection<Recipe>();
        public ICommand CreateRecipeCommand { get; }
        public ICommand SearchCommand { get; }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                SearchCommand?.Execute(value); // Automatically trigger the search command
            }
        }

        private ObservableCollection<Recipe> _filteredRecipes;
        public ObservableCollection<Recipe> FilteredRecipes
        {
            get => _filteredRecipes;
            set => SetProperty(ref _filteredRecipes, value);
        }

        public double ScreenWidth
        {
            get
            {
                return DeviceDisplay.Current.MainDisplayInfo.Width / DeviceDisplay.Current.MainDisplayInfo.Density;
            }
        }

        public MainViewModel()
        {
            App.MainViewModel = this;

            Title = "MealEase Recipes";

            CreateRecipeCommand = new RelayCommand(ExecuteCreateRecipeCommand);

            // Add example recipes to the collection
            Recipes.Add(new Recipe
            {
                RecipeName = "Recipe 1",
                RecipeImage = "https://www.eatingwell.com/thmb/P3WwF1tZ1iFaOvKQDnJthqT4kdY=/1500x0/filters:no_upscale():max_bytes(150000):strip_icc()/Quick-Cooking-Oats-0175-1-b7bb0c5ed67a43569c1d7bae7a75ae7a.jpg",
                MealType = "Breakfast",
                CookingTime = 10,
                Equipment = "Stove",
                Ingredients = "Ingredient 1, Ingredient 2, Ingredient 3",
                Steps = "Step 1, Step 2, Step 3"
            });

            Recipes.Add(new Recipe
            {
                RecipeName = "Recipe 2",
                RecipeImage = "https://www.thespruceeats.com/thmb/cQD_JvUOD_OzgrQkF0cUiqobkyE=/1500x0/filters:no_upscale():max_bytes(150000):strip_icc()/vegetarian-bean-and-rice-burrito-recipe-3378550-9_preview-5b2417e1ff1b780037a58cda.jpeg",
                MealType = "Lunch",
                CookingTime = 20,
                Equipment = "Microwaves",
                Ingredients = "Ingredient 4, Ingredient 5, Ingredient 6",
                Steps = "Step 4, Step 5, Step 6"
            });

            Recipes.Add(new Recipe
            {
                RecipeName = "Recipe 3",
                RecipeImage = "https://www.acouplecooks.com/wp-content/uploads/2019/03/Mushroom-Pasta-007.jpg",
                MealType = "Dinner",
                CookingTime = 30,
                Equipment = "Oven",
                Ingredients = "Ingredient 7, Ingredient 8, Ingredient 9",
                Steps = "Step 7, Step 8, Step 9"
            });

            CreateRecipeCommand = new RelayCommand(ExecuteCreateRecipeCommand);
            SearchCommand = new Command<string>(ExecuteSearchCommand);
        }

        private void ExecuteSearchCommand(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                FilteredRecipes = new ObservableCollection<Recipe>(Recipes);
            }
            else
            {
                searchText = searchText.ToLower();
                FilteredRecipes = new ObservableCollection<Recipe>(
                    Recipes.Where(recipe =>
                        recipe.RecipeName.ToLower().Contains(searchText) ||
                        recipe.MealType.ToLower().Contains(searchText) ||
                        recipe.Equipment.ToLower().Contains(searchText)
                    ));
            }

            Debug.WriteLine($"Filtered Recipes Count: {FilteredRecipes.Count}");

            // Check if there's a newly created recipe that matches the search criteria
            var newRecipeMatchingSearch = Recipes.FirstOrDefault(recipe =>
                recipe.RecipeName.ToLower().Contains(searchText) ||
                recipe.MealType.ToLower().Contains(searchText) ||
                recipe.Equipment.ToLower().Contains(searchText)
            );

            if (newRecipeMatchingSearch != null && !FilteredRecipes.Contains(newRecipeMatchingSearch))
            {
                FilteredRecipes.Add(newRecipeMatchingSearch);
                Debug.WriteLine("Newly created recipe added to FilteredRecipes");
            }
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

        private async void ExecuteCreateRecipeCommand()
        {
            await Shell.Current.GoToAsync(nameof(CreatePage));
        }


    }
}

