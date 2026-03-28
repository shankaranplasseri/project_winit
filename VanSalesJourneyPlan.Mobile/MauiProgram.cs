using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using VanSalesJourneyPlan.Mobile.Services;
using VanSalesJourneyPlan.Mobile.Views;

namespace VanSalesJourneyPlan.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            })
            .Services
            .AddSingleton<IApiClient, ApiClient>()
            .AddSingleton<ISecureStorageService, SecureStorageService>()
            .AddSingleton<ILocalCacheService, LocalCacheService>()
            .AddSingleton<IAuthenticationService, AuthenticationService>()
            .AddSingleton<IJourneyPlanService, JourneyPlanService>()
            .AddSingleton<LoginPage>()
            .AddSingleton<JourneyPlanListPage>()
            .AddSingleton<PlanDetailPage>()
            .AddSingleton<VisitLogPage>();

        var app = builder.Build();
        
        // Store service provider for later access
        ServiceHelper.ServiceProvider = app.Services;
        
        return app;
    }
}
