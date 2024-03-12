global using App.Models;
global using Plugin.LocalNotification;
global using System.Globalization;

using System.Threading.Tasks;
using System.Threading;
using Microsoft.Maui;
using static System.Net.Mime.MediaTypeNames;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;


namespace App.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        public readonly HttpClient httpClient;

        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        [ObservableProperty]
        bool isBusy;
        public bool IsNotBusy => !IsBusy;


        [NotifyPropertyChangedFor(nameof(IsNotRefreshing))]
        [ObservableProperty]
        bool isRefreshing;
        public bool IsNotRefreshing => !IsRefreshing;

        [ObservableProperty]
        static List<EventSchedule> events = new();


        [ObservableProperty]
        static bool notifications = false;

        [ObservableProperty]
        static EventSchedule firstEvent = new();
        [ObservableProperty]
        static EventSchedule secondEvent = new();
        [ObservableProperty]
        static EventSchedule thirdEvent = new();
        [ObservableProperty]
        static EventSchedule nextEvent = new();
        [ObservableProperty]
        static double percentageCompletion = 0.0;


        //[ObservableProperty]
        //static EventSchedule firstEvent = new();
        [ObservableProperty]
        static bool hasFirst = false;
        [ObservableProperty]
        static bool hasSecond = false;
        [ObservableProperty]
        static bool hasThird = false;
        [ObservableProperty]
        static bool hasMore = false;
        [ObservableProperty]
        static int howManyMore = 0;

        [ObservableProperty]
        static CalendarDay selectedDay = new();

        [ObservableProperty]
        static ApplicationUser user = new ApplicationUser();

        public BaseViewModel()
        {
                IsBusy = true;
            httpClient = new HttpClient();
            httpClient.BaseAddress = new("https://pzfgs7sq-7227.uks1.devtunnels.ms");
            IsBusy = false;


        }
        [ObservableProperty]
        public static List<CalendarDay> days = new List<CalendarDay>();
        [ObservableProperty]
        public static DateTime date = DateTime.Now;



        [NotifyPropertyChangedFor(nameof(HasNoGroups))]
        [ObservableProperty]
        bool hasGroups = true;
        public bool HasNoGroups => !HasGroups;

 
        [RelayCommand]
        public async Task Refresh()
        {
                IsBusy = true;
            string id = Preferences.Default.Get("userId", "").ToString();
           Notifications = Preferences.Default.Get("notify", false);
            var result = await httpClient.GetAsync($"api/applicationuser/{id}");
            if (!result.IsSuccessStatusCode)
            {
                IsBusy = false;
                return;
            }
            else
            {
                User = await result.Content.ReadFromJsonAsync<ApplicationUser>();
                Events = User?.Groups?.SelectMany(g => g.Events).SelectMany(e => e.Schedules).ToList().OrderBy(x => x.StartDateTime).Reverse().ToList();
                
                //Events = User?.Groups?.First()?.Events?.First()?.Schedules;
                Events.ForEach(e => e.GroupEvent = User?.Groups?.SelectMany(g => g.Events).FirstOrDefault(g => g.Id == e.GroupEventId));
                Events.ForEach(e => e.GroupEvent.Group = User?.Groups?.FirstOrDefault(g => g.Id == e.GroupEvent.GroupId));
                //Events.ForEach(e => e.GroupEvent = User?.Groups?.First()?.Events?.First());
                var TodaysEvents = Events?.Where(e => e.DayOfWeek == (int)DateTime.Now.DayOfWeek).ToList();
                

                // Get the current date and time
                DateTime date = DateTime.Now;
                // Get the current day of the week as an integer (0 is Sunday, 6 is Saturday)
                int day = (int)date.DayOfWeek;
                // Subtract the current day from the date to get the date of the previous Sunday
                DateTime sunday = date.AddDays(-day);
                // Add one day to get the date of the Monday of the current week
                DateTime monday = sunday.AddDays(1);
                Days.Clear();
                for (int i = 0; i < 7; i++)
                {
                    var d = sunday.AddDays(i).DayOfWeek.ToString().Substring(0, 3);
                    var e = sunday.AddDays(i).Day;
                    var dow = (int)sunday.AddDays(i).DayOfWeek;

                    if (sunday.AddDays(i) == date)
                        Days.Add(new() { DayIndex = i, DayOfMonth = e, DayOfWeek = d, Color = "Grey", Events = Events.Where(e => e.DayOfWeek == dow).ToList(), Selected = true });
                    else
                        Days.Add(new() { DayIndex = i, DayOfMonth = e, DayOfWeek = d, Color = "Transparent", Events = Events.Where(e => e.DayOfWeek == dow).ToList() });
                }
                Days.ForEach(d=>d.GroupedEvents = d.Events.GroupBy(e => e.StartDateTime.Hour));
                foreach(var sday in Days)
                {
                    foreach(var ev in sday.GroupedEvents)
                    {
                        sday.ScheduleGroups.Add(new ScheduleGroup(ev.Key.ToString(), ev.ToList()));
                    }
                }
                //Selected day should be the day of the week
                SelectedDay = Days.FirstOrDefault(x => x.Selected);
                NextEvent  = Events?.Where(e => e.StartDateTime.TimeOfDay > DateTime.Now.TimeOfDay).FirstOrDefault(); 

                var RemainingEvents = TodaysEvents?.Where(e => e.StartDateTime.TimeOfDay > DateTime.Now.TimeOfDay).ToList();
                FirstEvent = RemainingEvents?.FirstOrDefault();
                HasFirst = FirstEvent != null;
                SecondEvent = RemainingEvents?.Skip(1).FirstOrDefault();
                HasSecond = SecondEvent != null;
                ThirdEvent = RemainingEvents?.Skip(2).FirstOrDefault();
                HasThird = ThirdEvent != null;
                HasMore = RemainingEvents?.Count > 3;
                HowManyMore = (int)(RemainingEvents?.Count) - 3;


                PercentageCompletion = ((double)Events?.Where(e => e.StartDateTime < DateTime.Now).Count() / (double)Events?.Count());
                IsRefreshing = false;
                double total = (double)TodaysEvents?.Count();
                double completed = (double)TodaysEvents?.Where(e => e.StartDateTime.TimeOfDay < DateTime.Now.TimeOfDay).Count();
                PercentageCompletion = completed / total;
                IsBusy = false;

                await Shell.Current.GoToAsync("///Tabs");
            }
        }

        [RelayCommand]
        public async Task SetNotification()
        {
                IsBusy = true;
            SelectedDay = Days.FirstOrDefault(x => x.Selected);
                IsBusy = false;
            await Shell.Current.GoToAsync("///Tabs/SchedulePage");
            //await Toast.Make($" Creating Notification", ToastDuration.Short, 14).Show((new CancellationTokenSource()).Token);
            //var request = new NotificationRequest
            //{
            //    NotificationId = 100,
            //    Title = "Groups!",
            //    Subtitle = "You have groups today",
            //    Description = "You have groups today",
            //    BadgeNumber = 45,
            //    ReturningData = "Dummy data",
            //    CategoryType = NotificationCategoryType.Alarm,
            //    Schedule =
            //        {
            //            NotifyTime = DateTime.Now.AddSeconds(10),
            //            //NotifyRepeatInterval = TimeSpan.FromSeconds(5), 
            //            //RepeatType = NotificationRepeat.TimeInterval
            //        },
            //};
            //await LocalNotificationCenter.Current.Show(request);
            //await Toast.Make($"  Notification Created", ToastDuration.Short, 14).Show((new CancellationTokenSource()).Token);
        }

        [RelayCommand]
        public async Task SwitchNotification()
        {
            //Notifications = !Notifications;
                IsBusy = true;
            var s = Notifications ? "On" : "Off";
            await Toast.Make($"Notification turned {s}", ToastDuration.Short, 14).Show((new CancellationTokenSource()).Token);
            if(Notifications)
            {

                Preferences.Default.Set("notify", true);
                await LocalNotificationCenter.Current.Show(new NotificationRequest
                {
                    NotificationId = 99999,
                    Title = $"Notification",
                    Subtitle = $"Turning on notifications",
                    Description = $"Groups is looking at your groups and turning on notifications for each scheduled event",
                    CategoryType = NotificationCategoryType.Alarm,
                    Schedule =
                    {
                        NotifyTime =  DateTime.Now,
                        NotifyRepeatInterval = TimeSpan.FromDays(7),
                    },
                });
                var log = "";
                foreach (var ev in Events)
                {
                    log += $"{ev.GroupEvent.Name} {ev.StartDateTime} {ev.GroupEvent.Group.Name} \n";
                    var aboutToStart = new NotificationRequest
                    {
                        NotificationId = ev.Id+1000,
                        Title = $"{ev.GroupEvent.Group.Name}",
                        Subtitle = $"{ev.GroupEvent.Name}",
                        Description = $"{ev.GroupEvent.Name} is about to start, please attend",
                        CategoryType = NotificationCategoryType.Alarm,
                        Schedule =
                    {
                        NotifyTime =  DateTime.Today.Add(ev.StartDateTime.TimeOfDay).AddMinutes(-15),
                        NotifyRepeatInterval = TimeSpan.FromDays(7),
                        RepeatType = NotificationRepeat.TimeInterval
                    },
                    };

                    var start = new NotificationRequest
                    {
                        NotificationId = ev.Id + 999,
                        Title = $"{ev.GroupEvent.Group.Name}",
                        Subtitle = $"{ev.GroupEvent.Name}",
                        Description = $"{ev.GroupEvent.Name} has started, please attend",
                        CategoryType = NotificationCategoryType.Alarm,
                        Schedule =
                    {
                        NotifyTime =  DateTime.Today.Add(ev.StartDateTime.TimeOfDay),
                        NotifyRepeatInterval = TimeSpan.FromDays(7),
                        RepeatType = NotificationRepeat.TimeInterval
                    },
                    };

                    var ended = new NotificationRequest
                    {
                        NotificationId = ev.Id +998,
                        Title = $"{ev.GroupEvent.Group.Name}",
                        Subtitle = $"{ev.GroupEvent.Name}",
                        Description = $"{ev.GroupEvent.Name} has ended",
                        CategoryType = NotificationCategoryType.Alarm,
                        Schedule =
                    {
                        NotifyTime =  DateTime.Today.Add(ev.EndDateTime.TimeOfDay),
                        NotifyRepeatInterval = TimeSpan.FromDays(7),
                        RepeatType = NotificationRepeat.TimeInterval
                    },
                    };

                    await LocalNotificationCenter.Current.Show(aboutToStart);
                    await LocalNotificationCenter.Current.Show(start);
                    await LocalNotificationCenter.Current.Show(ended);
                } 
                await LocalNotificationCenter.Current.Show(new NotificationRequest
                {
                    NotificationId = 99998,
                    Title = $"Notification",
                    Subtitle = $"Notifications turned on",
                    Description = $"Groups finished \n {log}",
                    CategoryType = NotificationCategoryType.Alarm,
                    Schedule =
                    {
                        NotifyTime =  DateTime.Now,
                        NotifyRepeatInterval = TimeSpan.FromDays(7),
                    },
                });
            }
            else
            {
                Preferences.Default.Set("notify", false);
                LocalNotificationCenter.Current.CancelAll();
            }
                IsBusy = false;
        }

        [RelayCommand]
        public async void SignOutAsync()
        {
                IsBusy = true;
            Preferences.Default.Remove("userId");
            Preferences.Default.Remove("notify");
            User = new();
            await Toast.Make($"You have been logged out, please sign in", ToastDuration.Short, 14).Show((new CancellationTokenSource()).Token);
            await Shell.Current.GoToAsync("///SignInPage");
                IsBusy = false;
        }
    }
}
