using TestAssignment.Resources.Utilities;

namespace TestAssignment.Resources.Pages;

public class ApartmentPage : BasePage
{
    private static readonly string PricingData = "xpath=//div[@data-plugin-in-point-id='BOOK_IT_SIDEBAR']";
    
    public static ApartmentPage Instance { get; } = new();

    public async Task<int> GetCleaningFee()
    {
        await ClosePopup();
        ExtentReportHolder.LogMessage("Getting the cleaning fee for the apartment...");
        var pricingSection = await Playwright.Instance.FindElement(PricingData);
        var pricingText = await Playwright.Instance.GetElementText(pricingSection!);
        
        var lines = pricingText!.Split('\n');
        var foundCleaningFeeLabel = false;

        foreach (var line in lines)
        {
            if (line.Contains("Cleaning fee"))
            {
                foundCleaningFeeLabel = true;
                continue;
            }

            if (!foundCleaningFeeLabel) continue;
            var pricePart = line.Split(' ').FirstOrDefault(s => s.StartsWith('₪'));
            if (pricePart is not null && int.TryParse(pricePart.TrimStart('₪'), out var cleaningFee)) 
                return cleaningFee;
            break;
        }
        
        throw new Exception("Cleaning fee not found or could not be parsed.");
    }

}