using Microsoft.Maui.Controls;
using System.Xml.XPath;

namespace MealEase_Recipes.Views;

public partial class LandingPage : ContentPage
{
    string key = "";
 
    public LandingPage()
	{
		InitializeComponent();
        key = Preferences.Get("UserKey", "");
  

    }
    protected override void OnAppearing()
    {
        base.OnAppearing();

       
        Console.WriteLine("Key " + key);

        Device.StartTimer(TimeSpan.FromSeconds(1), () =>
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (key != "")
                {
                    Console.WriteLine("Key is valid");
                    await Navigation.PushAsync(new MainPage());
                }
                else
                {
                    Console.WriteLine("Key  unvalid");
                    await Navigation.PushAsync(new Login());
                }
               
            });

            return false; // Stop the timer
        });

      
    }
}