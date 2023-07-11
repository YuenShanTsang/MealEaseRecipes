using Firebase.Auth;
using System.ComponentModel;

namespace MealEaseRecipes.ViewModels
{
    internal class RegisterViewModel : INotifyPropertyChanged
    {
        // Define firebase web api key
        public string webApiKey = "AIzaSyDvuH7qtc3dNZp9jC5o2-JNZC4lAu2hSf0";
        private INavigation _navigation;
        // defined email variable
        private string email;
        // defined password variable
        private string password;
        // Handler when input change in the xmal file
        public event PropertyChangedEventHandler PropertyChanged;
        // Email called we denifed before xmal file 
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
}

