using TestAssignment.Resources.Utilities;

namespace TestAssignment.Resources.Pages;

public class MainPage
{
    private const string BaseUrl = "https://www.airbnb.com/";
    private static readonly string DestinationInput = "#bigsearch-query-location-input";
    private static readonly string CheckInDate = "xpath=//div[normalize-space()='Check in']";
    private static readonly string CheckOutDate = "xpath=//div[normalize-space()='Check out']";

    public static MainPage Instance { get; } = new();

    public MainPage Navigate()
    {
        ExtentReportHolder.LogMessage("Opening the main page...");
        Playwright.Instance.Page?.GotoAsync(BaseUrl).GetAwaiter().GetResult();
        return this;
    }

    public MainPage ChooseDestination(string destination)
    {
        ExtentReportHolder.LogMessage($"Searching for destination: {destination}");
        Playwright.Instance.TypeToElement(DestinationInput, destination);
        return this;
    }
    
    private string GetDateSelector(string date) => $"div[data-testid='calendar-day-{date}'][data-is-day-blocked='false']";
    
    public MainPage ChooseDates(string dateFrom, string dateTo)
    {
        ExtentReportHolder.LogMessage($"Choosing dates from: {dateFrom} to: {dateTo}");

        Playwright.Instance.ClickOnElement(CheckInDate);
        Playwright.Instance.ClickOnElement(GetDateSelector(dateFrom));
    
        Playwright.Instance.ClickOnElement(CheckOutDate);
        Playwright.Instance.ClickOnElement(GetDateSelector(dateTo));
        
        return this;
    }
}