using Microsoft.Maui.Controls;
using VanSalesJourneyPlan.Mobile.Services;

namespace VanSalesJourneyPlan.Mobile.Views;

[QueryProperty(nameof(PlanItemId), "id")]
public partial class VisitLogPage : ContentPage
{
    private readonly IJourneyPlanService _journeyPlanService;
    private int _planItemId;

    public int PlanItemId
    {
        get => _planItemId;
        set => _planItemId = value;
    }

    public VisitLogPage()
    {
        InitializeComponent();
        _journeyPlanService = ServiceHelper.GetService<IJourneyPlanService>()!;
        VisitDatePicker.Date = DateTime.Now.Date;
        VisitTimePicker.Time = DateTime.Now.TimeOfDay;
    }

    private async void OnLogVisitClicked(object sender, EventArgs e)
    {
        StatusLabel.IsVisible = false;

        try
        {
            // Validate
            var visitDate = VisitDatePicker.Date;

            string? visitTime = null;
            if (VisitTimePicker.Time != TimeSpan.Zero)
            {
                visitTime = VisitTimePicker.Time.ToString(@"hh\:mm");
            }

            decimal? salesAmount = null;
            if (decimal.TryParse(SalesAmountEntry.Text, out var amount))
            {
                salesAmount = amount;
            }

            var notes = NotesEditor.Text;

            LogVisitButton.IsEnabled = false;

            var (success, created) = await _journeyPlanService.LogVisitAsync(
                _planItemId,
                visitDate,
                visitTime,
                notes,
                salesAmount
            );

            if (success && created)
            {
                await DisplayAlert("Success", "Visit logged successfully!", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                StatusLabel.Text = "Failed to log visit. Please try again.";
                StatusLabel.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Error: {ex.Message}";
            StatusLabel.IsVisible = true;
        }
        finally
        {
            LogVisitButton.IsEnabled = true;
        }
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
