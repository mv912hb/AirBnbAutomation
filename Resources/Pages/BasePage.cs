﻿using Microsoft.Playwright;
using TestAssignment.Resources.Utilities;

namespace TestAssignment.Resources.Pages;

public class BasePage
{
    private static readonly string CloseButton = "xpath=//button[@aria-label='Close']";

    protected async Task ClosePopup() // this method might be useful for every advertisment popup on airbnb
    {
        ExtentReportHolder.LogMessage("Checking for startup popup...");
        try
        {
            await Playwright.Instance.ClickOnElement(CloseButton);
        }
        catch (PlaywrightException)
        {
            ExtentReportHolder.LogMessage("Popup did not appear...");
        }
    }
}