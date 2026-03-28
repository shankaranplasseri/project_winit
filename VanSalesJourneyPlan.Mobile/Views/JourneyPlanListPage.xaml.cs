using Microsoft.Maui.Controls;
using VanSalesJourneyPlan.Mobile.Services;

namespace VanSalesJourneyPlan.Mobile.Views;

public partial class JourneyPlanListPage : ContentPage
{
    private readonly IJourneyPlanService _journeyPlanService;

    public JourneyPlanListPage()
    {
        InitializeComponent();
        _journeyPlanService = ServiceHelper.GetService<IJourneyPlanService>()!;
        LoadPlans();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadPlans();
    }

    private async Task LoadPlans()
    {
        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;
        PlansCollectionView.IsVisible = false;

        try
        {
            var (success, plans) = await _journeyPlanService.GetMyJourneyPlansAsync();

            if (success && plans != null && plans.Count > 0)
            {
                PlansCollectionView.ItemsSource = plans;
                PlansCollectionView.IsVisible = true;
                EmptyStateView.IsVisible = false;

                // Handle selection
                PlansCollectionView.SelectionChangedCommand = new Command<object>(OnPlanSelected);
            }
            else
            {
                EmptyStateView.IsVisible = true;
                PlansCollectionView.IsVisible = false;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load plans: {ex.Message}", "OK");
            EmptyStateView.IsVisible = true;
            PlansCollectionView.IsVisible = false;
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }

    private void OnPlanSelected(object obj)
    {
        if (obj is JourneyPlanDto plan)
        {
            Shell.Current.GoToAsync($"plandetail/{plan.JourneyPlanId}");
        }
    }

    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        await LoadPlans();
    }
}
