using System;
namespace MealEaseRecipes.Models
{
	public class Recipe
	{
        // The name of the recipe  
        public string RecipeName { get; set; }

        // The image URL or path for the recipe
        public string RecipeImage { get; set; }

        // The type of meal (e.g., breakfast, lunch, dinner)
        public string MealType { get; set; }

        // The cooking time in minutes
        public int CookTime { get; set; }

        // The required appliance for the recipe (e.g., oven, stove)
        public string Appliance { get; set; }

        // The ingredients required for the recipe
        public string Ingredients { get; set; }

        // The step-by-step instructions for preparing the recipe
        public string Steps { get; set; }
    }
}

