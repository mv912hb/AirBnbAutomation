﻿using TestAssignment.Resources.Utilities;

namespace TestAssignment.Resources.Pages;

public class MainPage
{
    private const string BaseUrl = "https://www.airbnb.com/";
    private static readonly string DestinationInput = "#bigsearch-query-location-input";
    private static readonly string CheckInDate = "xpath=//div[normalize-space()='Check in']";
    private static readonly string CheckOutDate = "xpath=//div[normalize-space()='Check out']";

    public static MainPage Instance { get; } = new();

    public async Task<MainPage> Navigate()
    {
        ExtentReportHolder.LogMessage("Opening the main page...");
        var page = Playwright.Instance.Page;
        if (page is null)
        {
            await Playwright.Instance.OpenBrowser();
            page = Playwright.Instance.Page;
        }
        await page!.GotoAsync(BaseUrl);
        return this;
    }

    public async Task<MainPage> ChooseDestination(string destination)
    {
        ExtentReportHolder.LogMessage($"Searching for destination: {destination}");
        await Playwright.Instance.TypeToElement(DestinationInput, destination);
        return this;
    }
    
    public async Task<MainPage> ChooseDates(string dateFrom, string dateTo)
    {
        ExtentReportHolder.LogMessage($"Choosing dates from: {dateFrom} to: {dateTo}");
        await Playwright.Instance.ClickOnElement(CheckInDate);
        //await Playwright.Instance.TypeToElement(DestinationInput, destination);
        return this;
    }
}