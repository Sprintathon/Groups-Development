using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace App
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
            Refresh();
        }


        public  void Refresh()
        {
            var bvm = new BaseViewModel();
            string id = Preferences.Default.Get("userId", "").ToString();
            if (string.IsNullOrEmpty(id))
            {
                Shell.Current.GoToAsync("///SignInPage");
                return;
            }
            bvm.Notifications = Preferences.Default.Get("notify", false);
            var result = bvm.httpClient.GetAsync($"api/applicationuser/{id}").Result;
            if (!result.IsSuccessStatusCode)
            {
                return;
            }
            else
            {
                bvm.User = result.Content.ReadFromJsonAsync<ApplicationUser>().Result;
                bvm.Events = bvm.User?.Groups?.SelectMany(g => g.Events).SelectMany(e => e.Schedules).ToList().OrderBy(x => x.StartDateTime).Reverse().ToList();
                //Events = User?.Groups?.First()?.Events?.First()?.Schedules;
                bvm.Events.ForEach(e => e.GroupEvent = bvm.User?.Groups?.SelectMany(g => g.Events).FirstOrDefault(g => g.Id == e.GroupEventId));
                bvm.Events.ForEach(e => e.GroupEvent.Group = bvm.User?.Groups?.FirstOrDefault(g => g.Id == e.GroupEvent.GroupId));
                //Events.ForEach(e => e.GroupEvent = User?.Groups?.First()?.Events?.First());
                var TodaysEvents = bvm.Events?.Where(e => e.DayOfWeek == (int)DateTime.Now.DayOfWeek).ToList();
               

                // Get the current date and time
                DateTime date = DateTime.Now;
                // Get the current day of the week as an integer (0 is Sunday, 6 is Saturday)
                int day = (int)date.DayOfWeek;
                // Subtract the current day from the date to get the date of the previous Sunday
                DateTime sunday = date.AddDays(-day);
                // Add one day to get the date of the Monday of the current week
                DateTime monday = sunday.AddDays(1);
                bvm.Days.Clear();
                for (int i = 0; i < 7; i++)
                {
                    var d = sunday.AddDays(i).DayOfWeek.ToString().Substring(0, 3);
                    var e = sunday.AddDays(i).Day;
                    var dow = (int)sunday.AddDays(i).DayOfWeek;

                    if (sunday.AddDays(i) == date)
                        bvm.Days.Add(new() { DayIndex = i, DayOfMonth = e, DayOfWeek = d, Color = "Grey", Events = bvm.Events.Where(e => e.DayOfWeek == dow).ToList(), Selected = true });
                    else
                        bvm.Days.Add(new() { DayIndex = i, DayOfMonth = e, DayOfWeek = d, Color = "Transparent", Events = bvm.Events.Where(e => e.DayOfWeek == dow).ToList() });
                }
                //Selected day should be the day of the week
                bvm.SelectedDay = bvm.Days.FirstOrDefault(x => x.Selected);
                bvm.IsRefreshing = false;
                bvm.Days.ForEach(d => d.GroupedEvents = d.Events.GroupBy(e => e.StartDateTime.Hour));
                foreach (var sday in bvm.Days)
                {
                    foreach (var ev in sday.GroupedEvents)
                    {
                        sday.ScheduleGroups.Add(new ScheduleGroup(ev.Key.ToString(), ev.ToList()));
                    }
                }

                bvm.NextEvent = bvm.Events?.Where(e => e.StartDateTime.TimeOfDay > DateTime.Now.TimeOfDay).FirstOrDefault();

                var RemainingEvents = TodaysEvents?.Where(e => e.StartDateTime.TimeOfDay > DateTime.Now.TimeOfDay).ToList();
                bvm.FirstEvent = RemainingEvents?.FirstOrDefault();
                bvm.HasFirst = bvm.FirstEvent != null;
                bvm.SecondEvent = RemainingEvents?.Skip(1).FirstOrDefault();
                bvm.HasSecond = bvm.SecondEvent != null;
                bvm.ThirdEvent = RemainingEvents?.Skip(2).FirstOrDefault();
                bvm.HasThird = bvm.ThirdEvent != null;
                bvm.HasMore = RemainingEvents?.Count > 3;
                bvm.HowManyMore = (int)(RemainingEvents?.Count) - 3;


                double total = (double)TodaysEvents?.Count();
                double completed = (double)TodaysEvents?.Where(e => e.StartDateTime.TimeOfDay < DateTime.Now.TimeOfDay).Count();
                bvm.PercentageCompletion = completed / total;
                Shell.Current.GoToAsync("///Tabs");
            }
        }

    }
}
