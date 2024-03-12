global using Shared.Models;
global using CommunityToolkit.Mvvm.Input;
global using System.Collections.ObjectModel;
global using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Maui;
using static System.Net.Mime.MediaTypeNames;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace App.ViewModels
{
    public partial class GroupsViewModel : BaseViewModel
    {
        [ObservableProperty]
        ObservableCollection<Group> groups = new();

        public  GroupsViewModel()
        {
            IsBusy = true;
            LoadGroups();
        }

        [RelayCommand]
        public async void LoadGroups()
        {
           IsRefreshing = true;
            var result = await httpClient.GetAsync("api/group");
            if (!result.IsSuccessStatusCode)
            {
                IsBusy = false;
                IsRefreshing = false;
                return;
            }
            else
            {
                Groups.Clear();
                foreach (var group in await result.Content.ReadFromJsonAsync<List<Group>>())
                {
                    Groups.Add(group);
                }
                IsBusy = false;
                IsRefreshing = false;
                return;
            }
        }

        [RelayCommand]
        public void Search()
        {
            IsSearching = !IsSearching;
        }

        [RelayCommand]
        async void Join(Group group)
        {
                IsBusy = true;
            await Toast.Make($" Addind you to {group.Name}", ToastDuration.Short, 14).Show((new CancellationTokenSource()).Token);
            var result = await httpClient.GetAsync($"api/applicationuser/AddTogroup/{User.Id}/{group.Id}");
            if (!result.IsSuccessStatusCode)
            {
                await Toast.Make($" failed to add you to {group.Name}", ToastDuration.Short, 14).Show((new CancellationTokenSource()).Token);
                IsBusy = false;
                return;
            }
            else
            {
                await Refresh();
                //await Shell.Current.GoToAsync("///Tabs");
                await Toast.Make($" Added you to {group.Name}", ToastDuration.Short, 14).Show((new CancellationTokenSource()).Token);

                HasGroups = User.Groups.Any();
                IsBusy = false;
            }
        }
     


        [NotifyPropertyChangedFor(nameof(IsNotSearching))]
        [ObservableProperty]
        bool isSearching;
        public bool IsNotSearching => !(IsSearching && IsNotBusy);
    }
}
