using System.ComponentModel;
using Firebase.Auth;
using MealEaseRecipes.Views;
using Newtonsoft.Json;

namespace MealEaseRecipes.ViewModels
{
    internal class LoginViewModel : INotifyPropertyChanged
    {
        private string _validationMessage;

        // Define firebase web api key
        public string webApiKey = "AIzaSyDvuH7qtc3dNZp9jC5o2-JNZC4lAu2hSf0";
        private INavigation _navigation;

        // defined email variable
        private string email;

        // defined password variable
        private string userPassword;

        // Handler when input change in the xmal file
        public event PropertyChangedEventHandler PropertyChanged;

        // RegisterBtn called we denifed before xmal file 
        public Command RegisterBtn { get; }

        // LoginBtn called we denifed before xmal file 
        public Command LoginBtn { get; }

        // Email called we denifed before xmal file 
        public string Email
        {
            get => email; set
            {
                email = value;
                RaisePropertyChanged("Email");
                UpdateValidationMessage();
            }
        }
        // UserPassword called we denifed before xmal file 
        public string UserPassword
        {
            get => userPassword; set
            {
                userPassword = value;
                RaisePropertyChanged("UserPassword");
                UpdateValidationMessage();
            }
        }

        public string ValidationMessage
        {
            get => _validationMessage;
            set
            {
                _validationMessage = value;
                RaisePropertyChanged("ValidationMessage");
            }
        }

        private void UpdateValidationMessage()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(UserPassword))
            {
                ValidationMessage = "Please enter both email and password.";
            }
            else
            {
                ValidationMessage = string.Empty;
            }
        }

        public LoginViewModel(INavigation navigation)
        {
            // assgined navigation
            this._navigation = navigation;
            // when register button click we define new method
            RegisterBtn = new Command(RegisterBtnTappedAsync);
            // when login button click we define new method
            LoginBtn = new Command(LoginBtnTappedAsync);
        }

        private async void LoginBtnTappedAsync(object obj)
        {

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(UserPassword))
            {
                await App.Current.MainPage.DisplayAlert("Alert", "Please enter both email and password.", "OK");
                return;
            }

            // Connect our application to firebase auth
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(webApiKey));
            try
            {
                // Firebase auth selected signinWtihEmailAndPassoword option
                var auth = await authProvider.SignInWithEmailAndPasswordAsync(Email, UserPassword);
                // After signin getfreshAuth data
                var content = await auth.GetFreshAuthAsync();
                // serilaze data 
                var serializedContent = JsonConvert.SerializeObject(content);
                // set shared pref to data
                Preferences.Set("FreshFirebaseToken", serializedContent);
                // navigate to main page
                await this._navigation.PushAsync(new MainPage());
            }
            catch (FirebaseAuthException ex)
            {
                if (ex.Reason == AuthErrorReason.WrongPassword)
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Invalid password. Please try again.", "OK");
                }
                else
                {
                    var errorMessage = ex.Reason == AuthErrorReason.UnknownEmailAddress ? "Invalid email. Please try again." : "Invalid email or password. Please try again.";
                    await App.Current.MainPage.DisplayAlert("Error", errorMessage, "OK");
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Alert", ex.Message, "OK");
                throw;
            }

        }

        private async void RegisterBtnTappedAsync(object obj)
        {
            // when the user click register button direct to register page
            await this._navigation.PushAsync(new RegisterPage());
        }

        private void RaisePropertyChanged(string v)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }
    }
}

