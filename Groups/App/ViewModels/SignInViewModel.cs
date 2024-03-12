global using App.Views;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace App.ViewModels
{
    public partial class SignInViewModel : BaseViewModel
    {
        [RelayCommand]
        public async void GoToSignUpAsync() => await Shell.Current.GoToAsync(nameof(SignUpPage));
        [RelayCommand]
        public async void SignInAsync()
        {
            IsBusy = true;
            var result = await httpClient.GetAsync($"api/ApplicationUser/login/{User.Email}/{User.Password}");
            if(!result.IsSuccessStatusCode)
            {
                if(result.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    //await DisplayAlert("Error", "Your Email/Username or Password is not correct", "Try again");

                    bool answer  = await Application.Current.MainPage.DisplayAlert($"Error", "Your Email/Username or Password is not correct", "Yes", "No");
                    Debug.WriteLine("Answer: " + answer);
                }
                IsBusy = false;
                return;
            }
            else
            {
                ApplicationUser user = await result.Content.ReadFromJsonAsync<ApplicationUser>();
                //save token in shared preferences
                Preferences.Default.Set("userId", user.Id);
                Preferences.Default.Set("notify", false);
                await Refresh();
                IsBusy = false;
            }
        } 

        public SignInViewModel()
        {
            //    IsBusy = true;
            //if (Preferences.Default.ContainsKey("userId"))
            //{
            //    Refresh();
            //    IsBusy = false;
            //}
            IsBusy = false;

        }



    }
}
