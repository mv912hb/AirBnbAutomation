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
    private static readonly string NotExactDays = "xpath=//div[@id='panel--tabs--0']//label[3]";

    public static MainPage Instance { get; } = new();

    /// <summary>
    /// Navigates to the Airbnb main page and closes startup popup
    /// </summary>
    private async Task Navigate()
    {
        ExtentReportHolder.LogMessage("Opening the main page...");
        if (Playwright.Instance.Page is null) return;
        await Playwright.Instance.Page.GotoAsync(BaseUrl);
        await ClosePopup();
    }

    /// <summary>
    /// Chooses the destination
    /// </summary>
    /// <param name="destination">The destination to type into the search field</param>
    private async Task ChooseDestination(string? destination)
    {
        Thread.Sleep(3000);
        ExtentReportHolder.LogMessage($"Searching for destination: {destination}");
        await Playwright.Instance.TypeToElement(DestinationInput, destination);
    }

    /// <summary>
    /// Constructs a selector for a date element
    /// </summary>
    /// <param name="date">The date value to create a selector for</param>
    /// <returns>A string representing the selector for the choosen date</returns>
    private string GetDateSelector(string date) => $"div[data-testid='calendar-day-{date}'][data-is-day-blocked='false']";

    /// <summary>
    /// Chooses check-in and check-out dates
    /// </summary>
    /// <param name="dateFrom">The start date for the reservation</param>
    /// <param name="dateTo">The end date for the reservation</param>
    private async Task ChooseDates(string dateFrom, string dateTo)
    {
        ExtentReportHolder.LogMessage($"Choosing dates from: {dateFrom} to: {dateTo}");

        await Playwright.Instance.ClickOnElement(CheckInDate);
        await Playwright.Instance.ClickOnElement(GetDateSelector(dateFrom));

        await Playwright.Instance.ClickOnElement(CheckOutDate);
        await Playwright.Instance.ClickOnElement(GetDateSelector(dateTo));

        await Playwright.Instance.ClickOnElement(NotExactDays);
    }

    /// <summary>
    /// Adds a number of adults and children to the guest count
    /// </summary>
    /// <param name="adults">The number of adults to add</param>
    /// <param name="children">The number of children to add</param>
    private async Task AddGuests(int adults, int children)
    {
        ExtentReportHolder.LogMessage($"Adding guests... Adults: {adults}, children: {children}");

        await Playwright.Instance.ClickOnElement(Guests);

        for (var i = 0; i < adults; i++) await Playwright.Instance.ClickOnElement(AddAdult);
        for (var i = 0; i < children; i++) await Playwright.Instance.ClickOnElement(AddChildren);
    }

    /// <summary>
    /// Clicks the search button
    /// </summary>
    private async Task Search()
    {
        ExtentReportHolder.LogMessage("Pressing on search button");
        await Playwright.Instance.ClickOnElement(SearchButton);
    }

    /// <summary>
    /// Performs a complete search for apartments using specified criteria.
    /// </summary>
    public async Task SearchForApartments(string? destination, string dateFrom, string dateTo, int adults, int children)
    {
        await Navigate();
        await ChooseDestination(destination);
        await ChooseDates(dateFrom, dateTo);
        await AddGuests(adults, children);
        await Search();
    }
}