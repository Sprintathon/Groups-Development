namespace App.Views;

public partial class GroupsPage : ContentPage
{
	public GroupsPage()
	{
		InitializeComponent();
		BindingContext = new GroupsViewModel();
	}
}