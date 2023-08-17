namespace MealEaseRecipes.Models
{
    /*
    The Favourite class represents an item in a favorite list. 
    It includes an Id field as the primary key, a UserId field to associate with the user,
    a recipeID field that references the recipe the user wants to add to their favorites, and a Key field for the recipe's Firebase key.
    */
    class Favourite
    {
        // The unique identifier of the recipe
        public string RecipeId { get; set; }

        // The Firebase key associated with the recipe
        public string Key { get; set; }

        // The user ID of the user who favorited the recipe
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

        // The Firebase key associated with the favorite recipe entry
        public string FavKey { get; set; }

        // Indicates whether the recipe is marked as a favorite
        public bool IsFavourite { get; set; }
    }
}
