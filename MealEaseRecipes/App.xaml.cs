using MealEaseRecipes.ViewModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Maui.Controls;

namespace MealEaseRecipes;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        //MainViewModel = new MainViewModel(Navigation);

        MainPage = new AppShell();
    }

    public static MainViewModel MainViewModel { get; internal set; }

}
