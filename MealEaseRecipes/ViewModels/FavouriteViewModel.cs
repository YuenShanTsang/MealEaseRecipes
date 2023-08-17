using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using MealEaseRecipes.Models;

namespace MealEaseRecipes.ViewModels
{
    // The FavouriteViewModel class represents the ViewModel for the user's favorite recipes page.
    // It provides the necessary properties and methods to display and manage the user's favorite recipes.
    public class FavouriteViewModel : ObservableObject
    {
        // Collection of favorite recipes
        private ObservableCollection<Recipe> _favoriteRecipes;
        public ObservableCollection<Recipe> FavoriteRecipes
        {
            get => _favoriteRecipes;
            set => SetProperty(ref _favoriteRecipes, value);
        }

        // Constructor for initializing the ViewModel
        public FavouriteViewModel()
        {
            // Initialize the collection of favorite recipes
            FavoriteRecipes = new ObservableCollection<Recipe>();
        }
    }
}
