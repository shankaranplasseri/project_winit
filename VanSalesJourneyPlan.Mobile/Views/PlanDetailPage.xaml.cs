using Microsoft.Maui.Controls;
using VanSalesJourneyPlan.Mobile.Services;

namespace VanSalesJourneyPlan.Mobile.Views;

[QueryProperty(nameof(PlanId), "id")]
public partial class PlanDetailPage : ContentPage
{
    private readonly IJourneyPlanService _journeyPlanService;
    private int _planId;
    private JourneyPlanDto? _currentPlan;

    public int PlanId
    {
        get => _planId;
        set
        {
            _planId = value;
            LoadPlanDetails();
        }
    }

    public PlanDetailPage()
    {
        InitializeComponent();
        _journeyPlanService = ServiceHelper.GetService<IJourneyPlanService>()!;
    }

    private async Task LoadPlanDetails()
    {
        if (_planId == 0) return;

        LoadingIndicator.IsRunning = true;
        LoadingIndicator.IsVisible = true;

        try
        {
            var (success, plan) = await _journeyPlanService.GetJourneyPlanDetailsAsync(_planId);

            if (success && plan != null)
            {
                _currentPlan = plan;
                PlanTitleLabel.Text = plan.Title;
                PlanDateLabel.Text = $"Date: {plan.PlanDate}";
                CustomersCollectionView.ItemsSource = plan.Items;
            }
            else
            {
                await DisplayAlert("Error", "Failed to load plan details", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error loading plan: {ex.Message}", "OK");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }

    private async void OnLogVisitClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is JourneyPlanItemDto item)
        {
            // Navigate to visit log page
            await Shell.Current.GoToAsync($"visitlog/{item.JourneyPlanItemId}");
        }
    }
}
