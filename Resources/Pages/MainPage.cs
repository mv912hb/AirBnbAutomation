using OpenQA.Selenium;
using TestAssignment.Resources.Utilities;

namespace TestAssignment.Resources.Pages;

public class MainPage : BasePage
{
    private const string BaseUrl = "https://www.airbnb.com/";
    private static readonly By DestinationInput = By.Id("bigsearch-query-location-input");
    private static readonly By CheckInDate = By.XPath("//div[normalize-space()='Check in']");
    private static readonly By CheckOutDate = By.XPath("//div[normalize-space()='Check out']");
    private static readonly By Guests = By.XPath("//div[normalize-space()='Who']");
    private static readonly By AddAdult = By.XPath("//button[@data-testid='stepper-adults-increase-button']");
    private static readonly By AddChildren = By.XPath("//button[@data-testid='stepper-children-increase-button']");
    private static readonly By SearchButton = By.XPath("//button[@data-testid='structured-search-input-search-button']");

    public static MainPage Instance { get; } = new();

    public MainPage Navigate()
    {
        ExtentReportHolder.LogMessage("Opening the main page...");
        Selenium.Instance.NavigateToUrl(BaseUrl);
        ClosePopup();
        return this;
    }

    public MainPage ChooseDestination(string destination)
    {
        ExtentReportHolder.LogMessage($"Searching for destination: {destination}");
        Selenium.Instance.TypeToElement(DestinationInput, destination);
        return this;
    }

    private By GetDateSelector(string date) => By.XPath($"//div[@data-testid='calendar-day-{date}']");

    public MainPage ChooseDates(string dateFrom, string dateTo)
    {
        ExtentReportHolder.LogMessage($"Choosing dates from: {dateFrom} to: {dateTo}");

        Selenium.Instance.ClickOnElement(CheckInDate);
        Selenium.Instance.ClickOnElement(GetDateSelector(dateFrom));

        Selenium.Instance.ClickOnElement(CheckOutDate);
        Selenium.Instance.ClickOnElement(GetDateSelector(dateTo));
        return this;
    }

    public MainPage AddGuests(int adults, int children)
    {
        ExtentReportHolder.LogMessage($"Adding guests... Adults: {adults}, children: {children}");

        Selenium.Instance.ClickOnElement(Guests);

        for (var i = 0; i < adults; i++) Selenium.Instance.ClickOnElement(AddAdult);
        for (var i = 0; i < children; i++) Selenium.Instance.ClickOnElement(AddChildren);
        return this;
    }

    public void Search()
    {
        ExtentReportHolder.LogMessage("Pressing on search button");
        Selenium.Instance.ClickOnElement(SearchButton);
    }
}