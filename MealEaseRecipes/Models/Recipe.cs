﻿namespace MealEaseRecipes.Models
{
    // The Recipe class represents a recipe entity with attributes that define various properties of a recipe.
    // These attributes include the name of the recipe, URL or path of the recipe's image,
    // type of meal the recipe belongs to, cooking time in minutes, required appliance or equipment for preparing the recipe,
    // list of ingredients required and step-by-step instructions for preparing the recipe.
    // These attributes provide a comprehensive representation of a recipe,
    // enabling the application to handle recipe-related functionalities effectively.
    public class Recipe
    {
        // The Firebase key associated with the recipe
        public string Key { get; set; }

        // The user ID of the user who created the recipe
        public string UserId { get; set; }

        // The name of the recipe  
        public string RecipeName { get; set; }

        // The image URL or path for the recipe
        public string Image { get; set; }

        // The type of meal (e.g., breakfast, lunch, dinner)
        public string MealType { get; set; }

        // The cooking time in minutes
        public int CookingTime { get; set; }

        // The required equipment for the recipe (e.g., oven, stove)
        public string Equipment { get; set; }

        // The ingredients required for the recipe
        public string Ingredients { get; set; }

        // The step-by-step instructions for preparing the recipe
        public string Steps { get; set; }

        // Indicates whether the recipe is marked as a favorite
        public bool IsFavourite { get; set; }

        // The Firebase key associated with the favorite recipe entry
        public string FavKey { get; set; }
    }
}
