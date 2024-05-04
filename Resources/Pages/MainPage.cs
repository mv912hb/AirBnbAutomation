using TestAssignment.Resources.Utilities;

namespace TestAssignment.Resources.Pages;

public class MainPage : BasePage
{
    private const string BaseUrl = "https://www.airbnb.com/";
    private static readonly string DestinationInput = "#bigsearch-query-location-input";
    private static readonly string CheckInDate = "xpath=//div[normalize-space()='Check in']";
    private static readonly string CheckOutDate = "xpath=//div[normalize-space()='Check out']";
    private static readonly string Guests = "xpath=//div[normalize-space()='Who']";
    private static readonly string AddAdult = "xpath=//button[@data-testid='stepper-adults-increase-button']";
    private static readonly string AddChildren = "xpath=//button[@data-testid='stepper-children-increase-button']";
    private static readonly string SearchButton = "xpath=//button[@data-testid='structured-search-input-search-button']";

    public static MainPage Instance { get; } = new();

    private async Task Navigate()
    {
        ExtentReportHolder.LogMessage("Opening the main page...");
        if (Playwright.Instance.Page is null) return;
        await Playwright.Instance.Page.GotoAsync(BaseUrl);
        await ClosePopup();
    }

    private async Task ChooseDestination(string? destination)
    {
        Thread.Sleep(3000);
        ExtentReportHolder.LogMessage($"Searching for destination: {destination}");
        await Playwright.Instance.TypeToElement(DestinationInput, destination);
    }

    private string GetDateSelector(string date) => $"div[data-testid='calendar-day-{date}'][data-is-day-blocked='false']";

    private async Task ChooseDates(string dateFrom, string dateTo)
    {
        ExtentReportHolder.LogMessage($"Choosing dates from: {dateFrom} to: {dateTo}");

        await Playwright.Instance.ClickOnElement(CheckInDate);
        await Playwright.Instance.ClickOnElement(GetDateSelector(dateFrom));

        await Playwright.Instance.ClickOnElement(CheckOutDate);
        await Playwright.Instance.ClickOnElement(GetDateSelector(dateTo));
    }

    private async Task AddGuests(int adults, int children)
    {
        ExtentReportHolder.LogMessage($"Adding guests... Adults: {adults}, children: {children}");

        await Playwright.Instance.ClickOnElement(Guests);

        for (var i = 0; i < adults; i++) await Playwright.Instance.ClickOnElement(AddAdult);
        for (var i = 0; i < children; i++) await Playwright.Instance.ClickOnElement(AddChildren);
    }

    private async Task Search()
    {
        ExtentReportHolder.LogMessage("Pressing on search button");
        await Playwright.Instance.ClickOnElement(SearchButton);
    }

    public async Task SearchForApartments(string? destination, string dateFrom, string dateTo, int adults, int children)
    {
        await Navigate();
        await ChooseDestination(destination);
        await ChooseDates(dateFrom, dateTo);
        await AddGuests(adults, children);
        await Search();
    }
}