using MealEase_Recipes.ViewModels;

namespace MealEase_Recipes.Views
{
    public partial class DetailsPage : ContentPage
    {
        public DetailsPage()
        {
            InitializeComponent();
        }

        public DetailsPage(DetailsViewModel vm) : this()
        {
            BindingContext = vm;
        }
    }
}
