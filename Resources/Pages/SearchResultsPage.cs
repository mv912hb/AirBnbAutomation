using TestAssignment.Resources.Utilities;

namespace TestAssignment.Resources.Pages;

public class SearchResultsPage
{
    private static readonly string ApartmentCard = "xpath=//div[@data-testid='card-container']";
    private static readonly string NextPage = "xpath=//a[contains(@aria-label,'Next')]";
    
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