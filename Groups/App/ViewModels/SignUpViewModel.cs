namespace App.ViewModels
{
    public partial class SignUpViewModel : BaseViewModel
    {


        [RelayCommand]
        async void SignUp()
        {
                IsBusy = true;
            var result = await httpClient.PostAsJsonAsync("api/ApplicationUser", User);
            var content = await result.Content.ReadAsStringAsync();
            if (!result.IsSuccessStatusCode)
            {
                IsBusy = false;
                return;
            }
            else
            {
                //save token in shared preferences
                ApplicationUser user = await result.Content.ReadFromJsonAsync<ApplicationUser>();
                Preferences.Default.Set("userId", user.Id);
                Preferences.Default.Set("notify", false);
                await Shell.Current.GoToAsync("///Tabs");
                IsBusy = false;
            }

        }
        [RelayCommand]
        public async void GoToSignInAsync() => await Shell.Current.GoToAsync($"..");
    }
}
