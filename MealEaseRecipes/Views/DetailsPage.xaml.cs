using MealEaseRecipes.ViewModels;

namespace MealEaseRecipes.Views
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
