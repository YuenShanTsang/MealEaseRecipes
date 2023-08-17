namespace MealEaseRecipes.Models
{
    // The User class represents a user entity with attributes that define various properties of a user.
    // These attributes include the first name and last name of the user, username of the user,
    // email address of the user, password of the user, and date of birth of the user.
    // These attributes provide a comprehensive representation of a user,
    // enabling the application to handle user-related functionalities effectively.
    // It is used to store and access specific details of a user, facilitating authentication and profile management.
    public class User
    {
        // The Firebase key associated with the user
        public string Key { get; set; }

        // The Firebase Authentication key associated with the user
        public string FirebaseAuthKey { get; set; }

        // The URL or path of the user's profile photo
        public string Photo { get; set; }

        // The first name of the user
        public string FirstName { get; set; }

        // The last name of the user
        public string LastName { get; set; }

        // The email address of the user
        public string Email { get; set; }

        // The password of the user
        public string Password { get; set; }

        // The date of birth of the user
        public string DateOfBirth { get; set; }
    }
}
