using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using MealEaseRecipes.Models;
using System.ComponentModel;
using System.Xml.Linq;

namespace MealEaseRecipes.ViewModels;

internal class RegisterViewModel : INotifyPropertyChanged
{
    FirebaseClient firebaseClient = new Firebase.Database.FirebaseClient("https://mealeaserecipes-default-rtdb.firebaseio.com/");

    // Define firebase web api key
    public string webApiKey = "AIzaSyDMJKDqzzbPS7_NXVo3FDTXsm9MKqodG6E";
    private INavigation _navigation;
    // defined email variable
    private string email;
    // defined password variable
    private string password;

    private string firstName;

    private string lastName;
    // Handler when input change in the xmal file
    public event PropertyChangedEventHandler PropertyChanged;
    // Email called we denifed before xmal file 

    public string FirstName
    {
        get => firstName;
        set
        {
            firstName = value;
            RaisePropertyChanged("FirstName");
        }
    }

    public string LastName
    {
        get => lastName;
        set
        {
            lastName = value;
            RaisePropertyChanged("LastName");
        }
    }
    public string Email
    {
        get => email;
        set
        {
            email = value;
            RaisePropertyChanged("Email");
        }
    }
    // Password called we denifed before xmal file 
    public string Password
    {
        get => password; set
        {
            password = value;
            RaisePropertyChanged("Password");
        }
    }
    // RegisterUser called we denifed before xmal file 
    public Command RegisterUser { get; }

    private void RaisePropertyChanged(string v)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
    }

    public RegisterViewModel(INavigation navigation)
    {
        this._navigation = navigation;
        // when register button click we define new method
        RegisterUser = new Command(RegisterUserTappedAsync);
    }

    private async void RegisterUserTappedAsync(object obj)
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await App.Current.MainPage.DisplayAlert("Alert", "Please enter both email and password.", "OK");
            return;
        }

        try
        {
            // Connect our application to firebase auth
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(webApiKey));
            // Firebase auth selected createUserWithEmailAndPassword option
            var auth = await authProvider.CreateUserWithEmailAndPasswordAsync(Email, Password);
            // after register firebase give us token
            string token = auth.FirebaseToken;
            // check token if it is null give alert message else back to login page
            if (token != null)
                await App.Current.MainPage.DisplayAlert("Alert", "User Registered successfully", "OK");


            var response = await firebaseClient.Child("Users").PostAsync(new Models.User
            {
                Photo = "https://t3.ftcdn.net/jpg/01/65/63/94/360_F_165639425_kRh61s497pV7IOPAjwjme1btB8ICkV0L.jpg",
                FirebaseAuthKey = token,
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Password = Password,
                DateOfBirth = "01.01.2000"
            }
             );
            await firebaseClient.Child("Users").Child(response.Key).PutAsync(new Models.User
            {
                Key = response.Key,
                Photo = "https://t3.ftcdn.net/jpg/01/65/63/94/360_F_165639425_kRh61s497pV7IOPAjwjme1btB8ICkV0L.jpg",
                FirebaseAuthKey = token,
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Password = Password,
                DateOfBirth = "01.01.2000"
            });

            await this._navigation.PopAsync();
        }
        catch (FirebaseAuthException ex)
        {
            if (ex.Reason == AuthErrorReason.EmailExists)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Email already exists. Please choose a different email.", "OK");
            }
            else if (ex.Reason == AuthErrorReason.InvalidEmailAddress)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Invalid email address. Please enter a valid email.", "OK");
            }
            else if (ex.Reason == AuthErrorReason.WeakPassword)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Weak password. Please choose a stronger password.", "OK");
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", "Registration failed. Please try again.", "OK");
            }
        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("Alert", ex.Message, "OK");
            throw;
        }
    }
}