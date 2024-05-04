using System.Text.RegularExpressions;
using TestAssignment.Resources.Utilities;

namespace TestAssignment.Resources.Pages;

public class SearchResultsPage : BasePage
{
    private static readonly string ApartmentCard = "xpath=//div[@data-testid='price-availability-row']";
    private static readonly string PriceBreakdown = "xpath=//div[@aria-label='Price breakdown']";

    public static SearchResultsPage Instance { get; } = new();

    /// <summary>
    /// Searches for an apartment listed under a specified price
    /// </summary>
    /// <param name="price">Maximum price for nightly rental</param>
    public async Task FindApartmentUnderPrice(int price)
    {
        ExtentReportHolder.LogMessage($"Searching for apartment under the price {price} per night...");
        
        foreach (var element in await Playwright.Instance.FindElements(ApartmentCard))
        {
            var text = await Playwright.Instance.GetElementText(element);
            if (text is null || !GetPricePerNight(text, out var nightlyPrice) || nightlyPrice >= price) continue;
            await Playwright.Instance.ClickOnElement(element);
            return;
        }
        
        throw new Exception($"Apartment under price {price} is not found");
    }
    
    /// <summary>
    /// Retrieves the cleaning fee from the price breakdown provided
    /// </summary>
    /// <returns>The cleaning fee as an integer.</returns>
    public async Task<int> GetCleaningFee()
    {
        ExtentReportHolder.LogMessage("Retrieving the cleaning fee from the price breakdown...");

        try
        {
            var popupText = await Playwright.Instance.GetElementText(
                (await Playwright.Instance.Page!.WaitForSelectorAsync(PriceBreakdown))!);

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

    /// <summary>
    /// Parses the nightly price from an araptment description
    /// </summary>
    /// <param name="description">The text description from an apartment</param>
    /// <param name="price">Output parameter to store the parsed price</param>
    /// <returns>True if the price was parsed</returns>
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