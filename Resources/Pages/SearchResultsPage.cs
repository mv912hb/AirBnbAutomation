using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using TestAssignment.Resources.Utilities;

namespace TestAssignment.Resources.Pages;

public class SearchResultsPage : BasePage
{
    private static readonly By ApartmentCard = By.XPath("//div[@data-testid='price-availability-row']");
    private static readonly By PriceBreakdown = By.XPath("//div[@aria-label='Price breakdown']");

    public static SearchResultsPage Instance { get; } = new();

    public void FindApartmentUnderPrice(int price)
    {
        ExtentReportHolder.LogMessage($"Searching for apartment under the price {price} per night...");
        var driver = Selenium.Instance.Driver;
        var elements = Selenium.Instance.FindElements(ApartmentCard);
        foreach (var element in elements)
        {
            var text = Selenium.Instance.GetElementText(element);
            if (!GetPricePerNight(text, out var nightlyPrice) || nightlyPrice >= price) continue;
            new Actions(driver).MoveToElement(element).Perform();
            element.Click();
            return;
        }
    }

    public int GetCleaningFee()
    {
        ExtentReportHolder.LogMessage("Retrieving the cleaning fee from the price breakdown...");

        try
        {
            var price = Selenium.Instance.FindElement(PriceBreakdown);
            var popupText = Selenium.Instance.GetElementText(price);

            if (popupText is null) throw new Exception("Failed to retrieve text: Popup content is null.");
            
            var lines = popupText.Split('\n');

            var index = Array.IndexOf(lines, "Cleaning fee");
            if (index is -1 || index + 1 >= lines.Length) throw new Exception("Cleaning fee not found!");
            
            if (int.TryParse(Regex.Match(lines[index + 1], @"\d+").Value, out var cleaningFee)) return cleaningFee;

            throw new Exception("Failed to parse cleaning fee from the popup text.");
        }
        catch (TimeoutException ex)
        {
            throw new Exception($"Failed to find or retrieve text from the popup: {ex.Message}");
        }
    }

    private bool GetPricePerNight(string? description, out int price)
    {
        price = 0;
        var lines = description?.Split('\n');
        foreach (var line in lines!)
        {
            if (!line.Contains("night")) continue;
            var pricePart = line.Split(' ').FirstOrDefault(s => s.StartsWith('₪'));
            if (pricePart is not null && int.TryParse(pricePart.TrimStart('₪'), out price)) return true;
        }

        return false;
    }
}