using Microsoft.Playwright;
using TestAssignment.Resources.Utilities;

namespace TestAssignment.Resources;

public class Playwright
{
    private IBrowser? _browser;
    public static Playwright Instance { get; } = new();
    public IPage? Page { get; private set; }

    /// <summary>
    /// Opens a new browser
    /// </summary>
    /// <param name="browserType">Type of the browser to open</param>
    /// <returns>The active page of the launched browser</returns>
    public async Task OpenBrowser(string browserType = "Chromium")
    {
        var playwright = await Microsoft.Playwright.Playwright.CreateAsync();

        var browser = browserType switch
        {
            "Firefox" => await playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false }),
            _ => await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false })
        };

        _browser = browser;
        Page = await _browser.NewPageAsync();
        await Page.SetViewportSizeAsync(1920, 1080);
    }

    /// <summary>
    /// Closes the browser
    /// </summary>
    public async Task CloseBrowser()
    {
        ExtentReportHolder.LogMessage("Closing the browser...");
        if (Page is null) return;

        await Page.CloseAsync();
        Page = null;
        if (_browser is not null) await _browser.CloseAsync();
    }

    /// <summary>
    /// Finds elements on the current page
    /// </summary>
    /// <param name="selector">The selector to find elements</param>
    /// <param name="timeoutInSeconds">Timeout in seconds to wait for elements</param>
    /// <returns>A list of element handles.</returns>
    public async Task<List<IElementHandle>> FindElements(string selector, int timeoutInSeconds = 10)
    {
        var endTime = DateTime.UtcNow.AddSeconds(timeoutInSeconds);
        var elements = new List<IElementHandle>();
        var previousCount = 0;
    
        while (DateTime.UtcNow < endTime)
        {
            await Task.Delay(5000);
            var currentElements = await Page!.QuerySelectorAllAsync(selector);
            if (currentElements.Count != previousCount)
            {
                elements = currentElements.ToList();
                previousCount = elements.Count;
            }
            else
            {
                break;
            }
        }

        return elements;
    }
    
    /// <summary>
    /// Types to an element on the page
    /// </summary>
    /// <param name="selector">The selector of the element to type into</param>
    /// <param name="text">The text typed to the element</param>
    public async Task TypeToElement(string selector, string? text)
    {
        var element = await Page!.WaitForSelectorAsync(selector);
        if (element is not null) await element.FillAsync(text!);
    }

    /// <summary>
    /// Clicks on a specified element by locator
    /// </summary>
    /// <param name="selector">The selector of the element to click</param>
    public async Task ClickOnElement(string selector)
    {
        var element = await Page!.WaitForSelectorAsync(selector);
        if (element is not null) await element.ClickAsync();
    }
    
    /// <summary>
    /// Clicks on a specified element
    /// </summary>
    /// <param name="element">The element itself</param>
    public async Task ClickOnElement(IElementHandle element)
    {
        await element.WaitForElementStateAsync(ElementState.Stable);
        
        await Page!.EvaluateAsync("element => element.scrollIntoView({ behavior: 'smooth', block: 'center' })", element);
        await Task.Delay(500);
        
        await element.ClickAsync();
    }
    
    /// <summary>
    /// Retrieves the text content from an element
    /// </summary>
    /// <param name="element">The element to retrieve text from</param>
    /// <returns>The text of element</returns>
    public async Task<string?> GetElementText(IElementHandle element) => await element.InnerTextAsync();
}
