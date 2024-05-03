using System.Text.RegularExpressions;
using TestAssignment.Resources.Utilities;

namespace TestAssignment.Resources.Pages;

public class SearchResultsPage : BasePage
{
    private static readonly string ApartmentCard = "xpath=//div[@data-testid='price-availability-row']";
    private static readonly string PriceBreakdown = "xpath=//div[@aria-label='Price breakdown']";

    public static SearchResultsPage Instance { get; } = new();

    public async Task FindApartmentUnderPrice(int price)
    {
        ExtentReportHolder.LogMessage($"Searching for apartment under the price {price} per night...");
        do
        {
            var elements = await Playwright.Instance.FindElements(ApartmentCard);
            foreach (var element in elements)
            {
                var text = await Playwright.Instance.GetElementText(element);
                if (text is null || !GetPricePerNight(text, out var nightlyPrice) || nightlyPrice >= price) continue;
                await element.ClickAsync();
                return;
            }
        } while (true);
    }

    public async Task<int> GetCleaningFee()
    {
        ExtentReportHolder.LogMessage("Retrieving the cleaning fee from the price breakdown...");

        try
        {
            var popupText = await Playwright.Instance.GetElementText(
                await Playwright.GetPage()!.WaitForSelectorAsync(PriceBreakdown));

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