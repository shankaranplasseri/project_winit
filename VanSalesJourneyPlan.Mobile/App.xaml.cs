using Microsoft.Maui;
using Microsoft.Maui.Controls;
using VanSalesJourneyPlan.Mobile.Services;
using VanSalesJourneyPlan.Mobile.Views;

namespace VanSalesJourneyPlan.Mobile;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
    }

    protected override void OnStart()
    {
        // Check if user is already logged in
        CheckAuthentication();
    }

    protected override void OnResume()
    {
        CheckAuthentication();
    }

    private async void CheckAuthentication()
    {
        var secureStorage = ServiceHelper.GetService<ISecureStorageService>();
        if (secureStorage != null)
        {
            var isAuthenticated = await secureStorage.GetTokenAsync() != null;
            if (!isAuthenticated)
            {
                MainPage = new NavigationPage(new LoginPage());
            }
        }
    }
}

public static class ServiceHelper
{
    public static TService? GetService<TService>() where TService : class
    {
        return ServiceProvider?.GetService(typeof(TService)) as TService;
    }

    public static IServiceProvider? ServiceProvider { get; set; }
}

