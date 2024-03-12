using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace App.ViewModels
{
    public partial class ProfileViewModel : BaseViewModel
    {
        [ObservableProperty]
        ObservableCollection<Group> groups = new();


        public ProfileViewModel()
        {
            HasGroups = User.Groups.Any();
        }
        [RelayCommand]
        async void Leave(Group group)
        {
                IsBusy = true;
            await Toast.Make($"Removing you from {group.Name}", ToastDuration.Short, 14).Show((new CancellationTokenSource()).Token);
            var result = await httpClient.GetAsync($"api/applicationuser/RemoveFromGroup/{User.Id}/{group.Id}");
            if (!result.IsSuccessStatusCode)
            {
                await Toast.Make($" failed to remove you from {group.Name}", ToastDuration.Short, 14).Show((new CancellationTokenSource()).Token);
                IsBusy = false;
                return;
            }
            else
            {
                await Refresh();
                //await Shell.Current.GoToAsync("///Tabs");
                await Toast.Make($"Removed you from {group.Name}", ToastDuration.Short, 14).Show((new CancellationTokenSource()).Token);
                HasGroups = User.Groups.Any();
                IsBusy = false;
            }
        }

    }
}
