using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Firebase.Database;
using Firebase.Database.Query;
using MealEaseRecipes.Models;
using MealEaseRecipes.Views;
using MealEaseRecipes.Views.Recipe;

namespace MealEaseRecipes.ViewModels
{
    // The MainViewModel class represents the main ViewModel for the application.
    // It manages the main functionality of the application, including listing recipes, searching, favorites, and more.
    public partial class MainViewModel : ObservableObject
    {
        // Firebase Realtime Database client
        FirebaseClient firebaseClient = new Firebase.Database.FirebaseClient("https://mealeaserecipes-default-rtdb.firebaseio.com/");

        // Collection of user recipes
        public ObservableCollection<Recipe> UserList { get; set; } = new ObservableCollection<Recipe>();

        // Collection of all recipes
        public ObservableCollection<Recipe> Recipes { get; } = new ObservableCollection<Recipe>();

        private INavigation _navigation;

        private string _userId;

        // Commands for various actions
        public ICommand CreateRecipeCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand AddFavouritesCommand { get; }

        // Command for deleting a recipe
        public ICommand DeleteRecipeCommand => new Command<Recipe>(ExecuteDeleteRecipeCommand);

        // Command for editing a recipe
        public ICommand EditRecipeCommand => new Command<Recipe>(ExecuteEditRecipeCommand);

        // Search text property
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

        // Collection of favorite recipes
        private ObservableCollection<Recipe> _favoriteRecipes;
        public ObservableCollection<Recipe> FavoriteRecipes
        {
            get => _favoriteRecipes;
            set => SetProperty(ref _favoriteRecipes, value);
        }

        // Collection of filtered recipes based on search
        private ObservableCollection<Recipe> _filteredRecipes;
        public ObservableCollection<Recipe> FilteredRecipes
        {
            get => _filteredRecipes;
            set => SetProperty(ref _filteredRecipes, value);
        }

        // Get the screen width to fix the search bar width too short issue
        public double ScreenWidth
        {
            get
            {
                return DeviceDisplay.Current.MainDisplayInfo.Width / DeviceDisplay.Current.MainDisplayInfo.Density;
            }
        }

        // Constructor for initializing the ViewModel
        public MainViewModel(INavigation navigation)
        {
            App.MainViewModel = this;

            Title = "MealEase Recipes";

            _navigation = navigation;

            FetchFavoriteRecipes();

            // Subscribe to the Firebase "Recipes" node for updates
            var collection = firebaseClient
               .Child("Recipes")
               .AsObservable<Recipe>()
               .Subscribe((item) =>
               {
                   if (item.Object != null)
                   {
                       Console.WriteLine("User added");
                       if (!Recipes.Any(recipe => recipe.Key == item.Object.Key))
                       {
                           Recipes.Add(item.Object);
                           Debug.WriteLine($"Recipes Count: {Recipes.Count}");
                       }

                       // Only update FilteredRecipes if it's not null
                       if (FilteredRecipes != null)
                       {
                           ExecuteSearchCommand(SearchText);
                       }
                   }
                   else
                   {
                       Console.WriteLine("Object is null ");
                   }
               });

            CreateRecipeCommand = new RelayCommand(ExecuteCreateRecipeCommand);

            FilteredRecipes = null;

            SearchCommand = new Command<string>(ExecuteSearchCommand);

            AddFavouritesCommand = new Command<Recipe>(ExecuteAddFavouritesCommand);
            FavoriteRecipes = new ObservableCollection<Recipe>();
        }

        // Fetch favorite recipes from Firebase
        public async void FetchFavoriteRecipes()
        {
            var favorites = await firebaseClient.Child("Favourites").OnceAsync<Recipe>();
            FavoriteRecipes.Clear(); // Clear the existing list before adding new favorites
            foreach (var favorite in favorites)
            {
                FavoriteRecipes.Add(favorite.Object);
            }
        }

        // Execute the command to delete a recipe
        private async void ExecuteDeleteRecipeCommand(Recipe selectedRecipe)
        {
            var confirmed = await Application.Current.MainPage.DisplayAlert("Confirm Delete",
                $"Are you sure you want to delete the recipe '{selectedRecipe.RecipeName}'?", "Yes", "No");

            if (confirmed)
            {
                // Check if the recipe is a favorite, and if so, remove it from the FavoriteRecipes collection
                var existingFavorite = FavoriteRecipes.FirstOrDefault(recipe => recipe.Key == selectedRecipe.Key);
                if (existingFavorite != null)
                {
                    await RemoveFromFavorites(existingFavorite);
                }

                // Delete the recipe from Firebase "Recipes" node
                await firebaseClient.Child("Recipes").Child(selectedRecipe.Key).DeleteAsync();

                // Remove from local collections
                Recipes.Remove(selectedRecipe);
                FilteredRecipes.Remove(selectedRecipe);
                ExecuteSearchCommand(SearchText);
            }
        }

        // Execute the command to edit a recipe
        private async void ExecuteEditRecipeCommand(Recipe selectedRecipe)
        {
            Console.WriteLine($"Recipe {selectedRecipe.Key}");
            await Application.Current.MainPage.Navigation.PushAsync(new EditRecipePage(selectedRecipe));

            ExecuteSearchCommand(SearchText);
        }

        // Execute the search command
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
        }

        // Execute the command to add a recipe to favorites
        private async void ExecuteAddFavouritesCommand(Recipe selectedRecipe)
        {
            try
            {
                var existingFavorite = FavoriteRecipes.FirstOrDefault(recipe => recipe.RecipeName == selectedRecipe.RecipeName);

                if (existingFavorite != null)
                {
                    // Recipe is already in favorites, remove it
                    await RemoveFromFavorites(existingFavorite);
                }
                else
                {
                    var confirmed = await Application.Current.MainPage.DisplayAlert("Confirm Add to Favourites",
                        $"Are you sure you want to add the recipe '{selectedRecipe.RecipeName}' to your favourites?", "Yes", "No");

                    if (confirmed)
                    {
                        // Update the original recipe's IsFavourite property
                        var originalRecipe = Recipes.FirstOrDefault(recipe => recipe.Key == selectedRecipe.Key);
                        if (originalRecipe != null)
                        {
                            originalRecipe.IsFavourite = true;
                        }

                        var favourite = new Recipe
                        {
                            Key = selectedRecipe.Key,
                            UserId = selectedRecipe.UserId,
                            RecipeName = selectedRecipe.RecipeName,
                            Image = selectedRecipe.Image,
                            CookingTime = selectedRecipe.CookingTime,
                            Equipment = selectedRecipe.Equipment,
                            MealType = selectedRecipe.MealType,
                            Ingredients = selectedRecipe.Ingredients,
                            Steps = selectedRecipe.Steps,
                            IsFavourite = true
                        };

                        // Add the favorite to Firebase (or your preferred data storage)
                        var response = await firebaseClient.Child("Favourites").PostAsync(favourite);

                        // Update the FavKey property of the favorite recipe with the response key
                        favourite.FavKey = response.Key;

                        // Update the recipe in the "Favourites" node with the updated FavKey
                        await firebaseClient.Child("Favourites").Child(response.Key).PutAsync(favourite);

                        // Update the recipe in the "Recipes" node with the updated IsFavourite property
                        await firebaseClient.Child("Recipes").Child(selectedRecipe.Key).Child("IsFavourite").PutAsync(true);

                        // Refresh the favorite recipes in MainViewModel
                        FetchFavoriteRecipes();

                        // Update the image source to the heart full image
                        selectedRecipe.IsFavourite = true;

                        // Provide user feedback
                        await Application.Current.MainPage.DisplayAlert("Favourite Added", "This recipe has been added to your favourites.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        // Remove a recipe from favorites
        private async Task<bool> RemoveFromFavorites(Recipe recipe)
        {
            try
            {
                var favouriteToRemove = FavoriteRecipes.FirstOrDefault(favorite => favorite.FavKey == recipe.FavKey);

                if (favouriteToRemove != null)
                {
                    Debug.WriteLine("Found recipe to remove from favorites: " + favouriteToRemove.RecipeName);

                    // Remove from Firebase "Favourites" node
                    await firebaseClient.Child("Favourites").Child(favouriteToRemove.FavKey).DeleteAsync();

                    // Remove from the FavoriteRecipes collection
                    FavoriteRecipes.Remove(favouriteToRemove);

                    // Update the IsFavourite property of the original recipe
                    var originalRecipe = Recipes.FirstOrDefault(r => r.Key == recipe.Key);
                    if (originalRecipe != null)
                    {
                        originalRecipe.IsFavourite = false;

                        // Update the IsFavourite property of the recipe in the "Recipes" node
                        await firebaseClient.Child("Recipes").Child(recipe.Key).Child("IsFavourite").PutAsync(false);
                    }

                    // Provide user feedback
                    await Application.Current.MainPage.DisplayAlert("Removed from Favourites", "This recipe has been removed from your favourites.", "OK");

                    return true;
                }
                else
                {
                    Debug.WriteLine("Recipe not found in favorites: " + recipe.RecipeName);
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return false;
            }
        }

        [ObservableProperty]
        string title;

        // Command to navigate to the details page for a selected recipe
        [RelayCommand]
        public async Task GoToDetailsPage(Recipe selectedRecipe)
        {
            var detailsViewModel = new DetailsViewModel(selectedRecipe);
            var detailsPage = new DetailsPage(detailsViewModel);
            await Shell.Current.Navigation.PushAsync(detailsPage);
        }

        // Execute the command to navigate to the create recipe page
        private async void ExecuteCreateRecipeCommand()
        {
            await Shell.Current.GoToAsync(nameof(CreatePage));
        }


    }
}