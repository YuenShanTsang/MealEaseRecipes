using Firebase.Auth;
using Firebase.Database;
using MealEaseRecipes.Views;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace MealEaseRecipes.ViewModels;

internal class LoginViewModel : INotifyPropertyChanged
{
    FirebaseClient firebaseClient = new Firebase.Database.FirebaseClient("https://mealeaserecipes-default-rtdb.firebaseio.com/");
    public ObservableCollection<Models.User> UserList { get; set; } = new ObservableCollection<Models.User>();
    private string _validationMessage;

    // Define firebase web api key
    public string webApiKey = "AIzaSyDMJKDqzzbPS7_NXVo3FDTXsm9MKqodG6E";
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

    //public string AuthKeyFirstUser = "eyJhbGciOiJSUzI1NiIsImtpZCI6ImNmM2I1YWRhM2NhMzkxNTQ4ZDM1OTJiMzU5MjkyM2UzNjAxMmI5MTQiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vcmVjaXBlLWFwcC02NzlmNyIsImF1ZCI6InJlY2lwZS1hcHAtNjc5ZjciLCJhdXRoX3RpbWUiOjE2OTE3MDI5NDcsInVzZXJfaWQiOiJLNUlnMHpaZFRmT2xNcUpIZzg5MFA3QlE5MnIxIiwic3ViIjoiSzVJZzB6WmRUZk9sTXFKSGc4OTBQN0JROTJyMSIsImlhdCI6MTY5MTcwMjk0NywiZXhwIjoxNjkxNzA2NTQ3LCJlbWFpbCI6InRlc3RAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOmZhbHNlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7ImVtYWlsIjpbInRlc3RAZ21haWwuY29tIl19LCJzaWduX2luX3Byb3ZpZGVyIjoicGFzc3dvcmQifX0.Bxqz2LbGROISanTlah2gwTDNXvMEJVDzQYjcMaxNoNwi7g8Eb6-2LUmB2XqelCT6khTcM87pfeB86ZtZxmXr5YMmvjNrD1d44HOh00mNBbWOtO0s9z4ndReBy1LGZq_w4yaxg5kGqIodRfxSdr0VCsle4zJk_Nw4cJtg09_KemkalSNx10wk3CdnXPuQIDXOjO8fBB9MfcqVbncP92K0OkRJcRBOHXffo6GFGtTMvU2VA_Ywp1sQ09QiG27wqKGB09cVx3r0NIqnx_vZCwtGj7jAtTpriunwXjWR4ZHr61meSQFV6DOjmTBfzXK_FRNjuGBFSYNJm5oCp5ttKJz64Q";

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
        Console.WriteLine("LoginViewModel is working");

        if (UserList.Count <= 0)
        {
            var collection = firebaseClient
            .Child("Users")
            .AsObservable<Models.User>()
            .Subscribe((item) =>
            {
                if (item.Object != null)
                {
                    Console.WriteLine("User added");
                    UserList.Add(item.Object);


                }
                else
                {
                    Console.WriteLine("Object is null ");
                }
            });
        }
        Console.WriteLine("UserList count " + UserList.Count);
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
        if (UserList.Count <= 0)
        {
            await App.Current.MainPage.DisplayAlert("Alert", "Data is loading try again", "OK");
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




            IEnumerable<Models.User> obsCollection = (IEnumerable<Models.User>)UserList;
            var list = new List<Models.User>(obsCollection);


            var user = list.Where(user => user.Email == content.User.Email).First();

            // set shared pref to data
            Preferences.Set("UserKey", user.Key);

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
        await this._navigation.PushAsync(new Register());
    }

    private void RaisePropertyChanged(string v)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
    }
}