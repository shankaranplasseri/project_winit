using Microsoft.Maui.Controls;
using VanSalesJourneyPlan.Mobile.Services;

namespace VanSalesJourneyPlan.Mobile.Views;

public partial class LoginPage : ContentPage
{
    private readonly IAuthenticationService _authService;

    public LoginPage()
    {
        InitializeComponent();
        _authService = ServiceHelper.GetService<IAuthenticationService>()!;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var username = UsernameEntry.Text;
        var password = PasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ErrorLabel.Text = "Username and password are required";
            ErrorLabel.IsVisible = true;
            return;
        }

        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;
        LoginButton.IsEnabled = false;

        try
        {
            var (success, message, user) = await _authService.LoginAsync(username, password);

            if (success && user != null)
            {
                // Navigate to journey plans page
                await Shell.Current.GoToAsync("journeyplans");
            }
            else
            {
                ErrorLabel.Text = message;
                ErrorLabel.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = $"Login error: {ex.Message}";
            ErrorLabel.IsVisible = true;
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
            LoginButton.IsEnabled = true;
        }
    }
}
