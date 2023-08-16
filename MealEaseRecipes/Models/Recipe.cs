namespace MealEaseRecipes.Models
{

    public class Recipe
    {
        public string Key { get; set; }

        public string UserId { get; set; }

        // The name of the recipe  
        public string RecipeName { get; set; }

        // The image URL or path for the recipe
        public string Image { get; set; }

        // The type of meal (e.g., breakfast, lunch, dinner)
        public string MealType { get; set; }

        // The cooking time in minutes
        public int CookingTime { get; set; }

        // The required Equipment for the recipe (e.g., oven, stove)
        public string Equipment { get; set; }

        // The ingredients required for the recipe
        public string Ingredients { get; set; }

        // The step-by-step instructions for preparing the recipe
        public string Steps { get; set; }
    }
}
