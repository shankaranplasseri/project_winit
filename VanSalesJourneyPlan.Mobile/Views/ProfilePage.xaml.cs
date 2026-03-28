using Microsoft.Maui.Controls;
using VanSalesJourneyPlan.Mobile.Services;

namespace VanSalesJourneyPlan.Mobile.Views;

public partial class ProfilePage : ContentPage
{
    private readonly IAuthenticationService _authService;

    public ProfilePage()
    {
        InitializeComponent();
        _authService = ServiceHelper.GetService<IAuthenticationService>()!;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadUserInfo();
    }

    private async Task LoadUserInfo()
    {
        try
        {
            var user = await _authService.GetCurrentUserAsync();
            if (user != null)
            {
                UsernameLabel.Text = user.Username;
                RoleLabel.Text = user.Role;
            }
        }
        catch
        {
            UsernameLabel.Text = "Error loading user";
        }
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert(
            "Logout",
            "Are you sure you want to logout?",
            "Yes",
            "No"
        );

        if (confirm)
        {
            await _authService.LogoutAsync();
            // Navigate to login
            Application.Current!.MainPage = new NavigationPage(new LoginPage());
        }
    }
}
