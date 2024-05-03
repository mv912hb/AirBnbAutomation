using OpenQA.Selenium;
using TestAssignment.Resources.Utilities;

namespace TestAssignment.Resources.Pages;

public class BasePage
{
    private static readonly By CloseButton = By.XPath("//button[@aria-label='Close']");

    protected void ClosePopup()
    {
        ExtentReportHolder.LogMessage("Checking for startup popup...");
        try
        {
            Selenium.Instance.ClickOnElement(CloseButton);
        }
        catch (Exception)
        {
            ExtentReportHolder.LogMessage("Popup did not appear...");
        }
    }
}