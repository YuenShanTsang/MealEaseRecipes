using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using MealEaseRecipes.Models;
using Plugin.Media.Abstractions;

namespace MealEaseRecipes.ViewModels
{
    public class CreateViewModel : INotifyPropertyChanged
    {

        // Realtime Database
        FirebaseClient firebaseClient = new Firebase.Database.FirebaseClient("https://mealeaserecipes-default-rtdb.firebaseio.com/");

        // Commands for image selection and form submission
        public ICommand SelectImageCommand { get; private set; }
        public ICommand SubmitCommand { get; private set; }

        // Event handler for property change notifications
        public event PropertyChangedEventHandler PropertyChanged;

        // User ID
        public string userId = "";

        // Property to determine if an image is not selected
        public bool IsNotImageSelected => !IsImageSelected;

        // Media file for selected image
        private MediaFile result;

        // Recipe Image
        private string _image;
        public string Image
        {
            get => _image;
            set
            {
                if (_image != value)
                {
                    _image = value;
                    OnPropertyChanged(nameof(Image));
                }
            }
        }

        // Recipe image selection
        private bool _isImageSelected = false;
        public bool IsImageSelected
        {
            get => _isImageSelected;
            set
            {
                if (_isImageSelected != value)
                {
                    _isImageSelected = value;
                    OnPropertyChanged(nameof(IsImageSelected));
                    OnPropertyChanged(nameof(IsNotImageSelected));
                }
            }
        }

        // Property for the Recipe Name
        private string _recipeName;
        public string RecipeName
        {
            get => _recipeName;
            set
            {
                if (_recipeName != value)
                {
                    _recipeName = value;
                    OnPropertyChanged(nameof(RecipeName));
                }
            }
        }

        // Properties for Meal Type radio buttons
        private bool _isBreakfast;
        public bool IsBreakfast
        {
            get => _isBreakfast;
            set
            {
                if (_isBreakfast != value)
                {
                    _isBreakfast = value;
                    OnPropertyChanged(nameof(IsBreakfast));
                }
            }
        }

        private bool _isLunch;
        public bool IsLunch
        {
            get => _isLunch;
            set
            {
                if (_isLunch != value)
                {
                    _isLunch = value;
                    OnPropertyChanged(nameof(IsLunch));
                }
            }
        }

        private bool _isDinner;
        public bool IsDinner
        {
            get => _isDinner;
            set
            {
                if (_isDinner != value)
                {
                    _isDinner = value;
                    OnPropertyChanged(nameof(IsDinner));
                }
            }
        }

        // Property for Cooking Time
        private int _cookingTime;
        public int CookingTime
        {
            get => _cookingTime;
            set
            {
                if (_cookingTime != value)
                {
                    _cookingTime = value;
                    OnPropertyChanged(nameof(CookingTime));
                }
            }
        }

        // Collection of equipment options
        private ObservableCollection<string> _equipmentList;
        public ObservableCollection<string> EquipmentList
        {
            get => _equipmentList;
            set
            {
                _equipmentList = value;
                OnPropertyChanged(nameof(EquipmentList));
            }
        }

        // Property for Selected Equipment
        private string _selectedEquipment;
        public string SelectedEquipment
        {
            get => _selectedEquipment;
            set
            {
                if (_selectedEquipment != value)
                {
                    _selectedEquipment = value;
                    OnPropertyChanged(nameof(SelectedEquipment));
                }
            }
        }

        // Property for Ingredients
        private string _ingredients;
        public string Ingredients
        {
            get => _ingredients;
            set
            {
                if (_ingredients != value)
                {
                    _ingredients = value;
                    OnPropertyChanged(nameof(Ingredients));
                }
            }
        }

        // Property for Steps
        private string _steps;
        public string Steps
        {
            get => _steps;
            set
            {
                if (_steps != value)
                {
                    _steps = value;
                    OnPropertyChanged(nameof(Steps));
                }
            }
        }

        // Constructor
        public CreateViewModel()
        {
            // Initialize SubmitCommand to call OnSubmitButtonClicked
            SubmitCommand = new Command(OnSubmitButtonClicked);

            // Get the user ID from preferences
            userId = Xamarin.Essentials.Preferences.Get("UserKey", "");

            // Initialize EquipmentList with equipment options
            EquipmentList = new ObservableCollection<string>
            {
                "Toaster",
                "Microwave",
                "Oven",
                "Frying pan",
                "Rice Cooker",
                "Blender",
                "Air fryer",
                "Pot",
                "Electric kettle"
            };

            // Initialize SelectImageCommand to handle image selection
            SelectImageCommand = new Command(async () =>
            {
                string imageUrl = await OnSelectImageButtonClicked();
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    Image = imageUrl;
                    IsImageSelected = true;
                }
                else
                {
                    // Display an error message if image upload fails
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to upload image.", "OK");
                }
            });

        }

        // Image Selection
        private async Task<string> OnSelectImageButtonClicked()
        {
            try
            {
                var photo = await Microsoft.Maui.Media.MediaPicker.PickPhotoAsync();

                if (photo == null)
                    return string.Empty; // User canceled the photo selection

                using (var stream = await photo.OpenReadAsync())
                {
                    var imagePath = $"images/{Guid.NewGuid().ToString("N")}.jpg";

                    var firebaseStorage = new FirebaseStorage("mealeaserecipes.appspot.com");
                    var response = await firebaseStorage.Child(imagePath).PutAsync(stream);

                    if (response != null)
                    {
                        Console.WriteLine("Image uploaded successfully. Download URL: " + response);
                        return response; // Return the download URL
                    }
                    else
                    {
                        Console.WriteLine("Failed to upload image");
                        return string.Empty; // Return an appropriate default value
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error selecting and uploading image: " + ex.Message);
                return string.Empty; // Return an appropriate default value
            }
        }

        // Recipe form submission validation
        private bool ValidateFormFields()
        {
            if (string.IsNullOrWhiteSpace(RecipeName))
                return false;

            if (!IsBreakfast && !IsLunch && !IsDinner)
                return false;

            if (CookingTime <= 0)
                return false;

            if (string.IsNullOrWhiteSpace(SelectedEquipment))
                return false;

            if (string.IsNullOrWhiteSpace(Ingredients))
                return false;

            if (string.IsNullOrWhiteSpace(Steps))
                return false;

            if (!IsImageSelected)
                return false;

            return true;
        }

        // Method called when the Submit button is clicked
        private async void OnSubmitButtonClicked()
        {
            if (!ValidateFormFields())
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please fill in all required fields before submitting the form.", "OK");
                return;
            }

            string imageUrl = Image;
            

            // Determine the meal type based on the selected radio button
            string mealType = "";

            if (IsBreakfast)
                mealType = "Breakfast";
            else if (IsLunch)
                mealType = "Lunch";
            else if (IsDinner)
                mealType = "Dinner";

            try
            {
                // Post the recipe data to the Firebase database
                var response = await firebaseClient.Child("Recipes").PostAsync(
                    new Recipe
                    {
                        Image = imageUrl,
                        RecipeName = RecipeName,
                        UserId = userId,
                        MealType = mealType,
                        CookingTime = CookingTime,
                        Equipment = SelectedEquipment,
                        Ingredients = Ingredients,
                        Steps = Steps
                    });

                // Update the recipe data with the generated key
                await firebaseClient.Child("Recipes").Child(response.Key).PutAsync(new Recipe
                {
                    Key = response.Key,
                    Image = imageUrl,
                    RecipeName = RecipeName,
                    UserId = userId,
                    MealType = mealType,
                    CookingTime = CookingTime,
                    Equipment = SelectedEquipment,
                    Ingredients = Ingredients,
                    Steps = Steps
                });

                Console.WriteLine("Response " + response.Key);

                // Check the status code to determine if the data was saved successfully
                if (response != null)
                {
                    // Display an alert indicating successful submission (optional)
                    string message = $"Recipe Name: {RecipeName}\nMeal Type: {mealType}\nCooking Time: {CookingTime}\nEquipment: {SelectedEquipment}\nIngredients: {Ingredients}\nSteps: {Steps}";
                    await Application.Current.MainPage.DisplayAlert("Successful Recipe Submission", message, "OK");

                    // Navigate back to the Main page
                    await Application.Current.MainPage.Navigation.PopAsync();
                }
                else
                {
                    Console.WriteLine("Failed to save data");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

        }

        // Method to raise the PropertyChanged event
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
