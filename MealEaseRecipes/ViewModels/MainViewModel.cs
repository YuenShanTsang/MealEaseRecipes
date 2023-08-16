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
    public partial class MainViewModel : ObservableObject
    {
        FirebaseClient firebaseClient = new Firebase.Database.FirebaseClient("https://mealeaserecipes-default-rtdb.firebaseio.com/");
        public ObservableCollection<Recipe> UserList { get; set; } = new ObservableCollection<Recipe>();
        public ObservableCollection<Recipe> Recipes { get; } = new ObservableCollection<Recipe>();

        private INavigation _navigation;

        private ICommand _addToFavoritesCommand;

        public ICommand CreateRecipeCommand { get; }
        public ICommand SearchCommand { get; }

        public ICommand DeleteRecipeCommand => new Command<Recipe>(ExecuteDeleteRecipeCommand);
        public ICommand EditRecipeCommand => new Command<Recipe>(ExecuteEditRecipeCommand);

        public ICommand AddFavouritesCommand => _addToFavoritesCommand ??= new Command<Recipe>(ExecuteAddToFavouritesCommand);


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

        public ObservableCollection<Recipe> FavoriteRecipes { get; } = new ObservableCollection<Recipe>();


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

        public MainViewModel(INavigation navigation)
        {
            App.MainViewModel = this;

            Title = "MealEase Recipes";

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

        }

        private async void ExecuteDeleteRecipeCommand(Recipe selectedRecipe)
        {
            var confirmed = await Application.Current.MainPage.DisplayAlert("Confirm Delete",
                $"Are you sure you want to delete the recipe '{selectedRecipe.RecipeName}'?", "Yes", "No");

            if (confirmed)
            {
                await firebaseClient.Child("Recipes").Child(selectedRecipe.Key).DeleteAsync();
                Recipes.Remove(selectedRecipe);
                FilteredRecipes.Remove(selectedRecipe);
                ExecuteSearchCommand(SearchText);
            }
        }

        private async void ExecuteEditRecipeCommand(Recipe selectedRecipe)
        {
            Console.WriteLine($"Recipe {selectedRecipe.Key}");
            await Application.Current.MainPage.Navigation.PushAsync(new EditRecipePage(selectedRecipe));

            ExecuteSearchCommand(SearchText);
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
        }

        private void ExecuteAddToFavouritesCommand(Recipe recipe)
        {
            if (!FavoriteRecipes.Contains(recipe))
            {
                FavoriteRecipes.Add(recipe);
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