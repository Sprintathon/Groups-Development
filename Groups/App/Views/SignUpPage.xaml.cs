global using CommunityToolkit.Mvvm.Input;

namespace App.Views;

public partial class SignUpPage : ContentPage
{
	public SignUpPage()
	{
		InitializeComponent();
		BindingContext = new SignUpViewModel();
    }
}