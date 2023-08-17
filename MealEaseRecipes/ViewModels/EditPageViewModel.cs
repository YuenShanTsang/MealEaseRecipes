using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using MealEaseRecipes.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace MealEaseRecipes.ViewModels;

class EditPageViewModel : INotifyPropertyChanged
{
    // Realtime Database
    FirebaseClient firebaseClient = new Firebase.Database.FirebaseClient("https://mealeaserecipes-default-rtdb.firebaseio.com/");

    public event PropertyChangedEventHandler PropertyChanged;

    public ICommand SelectImageCommand { get; private set; }
    public ICommand SubmitCommand { get; private set; }

    public string userId = "";

    // Recipe being edited
    public Recipe recipe { get; set; }

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
    public EditPageViewModel(INavigation navigation, Recipe recipe)
    {
        // Initialize SubmitCommand to call OnSubmitButtonClicked
        SubmitCommand = new Command(OnSubmitButtonClicked);

        // Command for selecting an image
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

        // Set meal type radio buttons based on recipe
        Console.WriteLine("Meal Type " + recipe.MealType);
        if (recipe.MealType.First() == 'B')
        {
            IsBreakfast = true;
        }
        else if (recipe.MealType.First() == 'L')
        {
            IsLunch = true;
        }
        else
        {
            IsDinner = true;
        }

        // Initialize properties with recipe data
        this.recipe = recipe;
        RecipeName = recipe.RecipeName;
        Image = recipe.Image;
        CookingTime = recipe.CookingTime;
        SelectedEquipment = recipe.Equipment;
        Ingredients = recipe.Ingredients;
        Steps = recipe.Steps;

        userId = Preferences.Get("UserKey", "");

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

    // Method called when the Submit button is clicked
    private async void OnSubmitButtonClicked()
    {
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
            // Update the recipe details in the Firebase database
            await firebaseClient.Child("Recipes").Child(this.recipe.Key).PutAsync(new Recipe
            {
                Key = this.recipe.Key,
                Image = Image,
                RecipeName = RecipeName,
                UserId = userId,
                MealType = mealType,
                CookingTime = CookingTime,
                Equipment = SelectedEquipment,
                Ingredients = Ingredients,
                Steps = Steps
            });

            Console.WriteLine("Response " + this.recipe.Key);


            // Display an alert indicating successful submission (optional)
            string message = $"Recipe Name: {RecipeName}\nMeal Type: {mealType}\nCooking Time: {CookingTime}\nEquipment: {SelectedEquipment}\nIngredients: {Ingredients}\nSteps: {Steps}";
            await Application.Current.MainPage.DisplayAlert("Form Submission", message, "OK");

            // Navigate back to the Main page
            await Application.Current.MainPage.Navigation.PopAsync();
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

