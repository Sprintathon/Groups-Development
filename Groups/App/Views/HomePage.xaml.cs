namespace App.Views;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
		BindingContext = new HomeViewModel();
	}
}