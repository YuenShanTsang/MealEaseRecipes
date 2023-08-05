using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using MealEaseRecipes.Models;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace MealEaseRecipes.ViewModels
{
    public class CreateViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Command for selecting an image
        public ICommand SelectImageCommand { get; private set; }

        private ImageSource _recipeImage;
        public ImageSource RecipeImage
        {
            get => _recipeImage;
            set
            {
                if (_recipeImage != value)
                {
                    _recipeImage = value;
                    OnPropertyChanged(nameof(RecipeImage));
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



        // Command for the Submit button
        public ICommand SubmitCommand { get; private set; }

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

        // Constructor
        public CreateViewModel()
        {
            // Initialize SubmitCommand to call OnSubmitButtonClicked
            SubmitCommand = new Command(OnSubmitButtonClicked);


            SelectImageCommand = new Command(async () => await SelectImage());

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

        private async Task SelectImage()
        {
            var selectedImage = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Select an image"
            });

            if (selectedImage != null)
            {
                using (var imageStream = await selectedImage.OpenReadAsync())
                {
                    // Convert the image stream to a byte array
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        await imageStream.CopyToAsync(memoryStream);
                        byte[] imageBytes = memoryStream.ToArray();

                        // Convert the byte array to an ImageSource
                        ImageSource imageSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));

                        // Set the RecipeImage property to the selected ImageSource
                        RecipeImage = imageSource;
                    }
                }
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

            // Create a Recipe object with the collected data
            Recipe newRecipe = new Recipe
            {
                RecipeImage = RecipeImage,
                RecipeName = RecipeName,
                MealType = mealType,
                CookingTime = CookingTime,
                Equipment = SelectedEquipment,
                Ingredients = Ingredients,
                Steps = Steps
            };

            // Add the new recipe to the MainViewModel's collection of recipes
            App.MainViewModel.Recipes.Add(newRecipe);

            // Check if the new recipe matches the search criteria
            if (IsRecipeMatchingSearch(newRecipe))
            {
                // Add the new recipe to the FilteredRecipes collection in MainViewModel
                App.MainViewModel.FilteredRecipes.Add(newRecipe);
            }

            // Display an alert indicating successful submission (optional)
            string message = $"Recipe Name: {RecipeName}\nMeal Type: {mealType}\nCooking Time: {CookingTime}\nEquipment: {SelectedEquipment}\nIngredients: {Ingredients}\nSteps: {Steps}";
            await Application.Current.MainPage.DisplayAlert("Form Submission", message, "OK");

            await Application.Current.MainPage.Navigation.PopAsync();
        }

        private bool IsRecipeMatchingSearch(Recipe recipe)
        {
            string searchText = App.MainViewModel.SearchText.ToLower();

            return recipe.RecipeName.ToLower().Contains(searchText) ||
                   recipe.MealType.ToLower().Contains(searchText) ||
                   recipe.Equipment.ToLower().Contains(searchText);
        }



        // Method to raise the PropertyChanged event
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

