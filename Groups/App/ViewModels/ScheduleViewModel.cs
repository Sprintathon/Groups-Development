namespace App.ViewModels
{
    public partial class ScheduleViewModel : BaseViewModel
    {
       
        [RelayCommand]
        async void RefreshSchedule()
        {
            await Refresh();
        }
        [RelayCommand]
        async void SelectDay(CalendarDay clickedDay)
        {
                IsBusy = true;
            SelectedDay = clickedDay;
            Days.ForEach(d => { d.Selected = false; d.Color = "Transparent"; });
            Days.Where(d => d.DayIndex == clickedDay.DayIndex).FirstOrDefault().Selected = true;
            Days.Where(d => d.DayIndex == clickedDay.DayIndex).FirstOrDefault().Color = "Grey";
            //await Shell.Current.GoToAsync($"///Tabs/Schedule?day={day.DayIndex}");
                IsBusy = false;
        }

    }
}
