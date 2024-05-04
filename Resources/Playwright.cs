using Microsoft.Playwright;
using TestAssignment.Resources.Utilities;

namespace TestAssignment.Resources;

public class Playwright
{
    private IBrowser? _browser;
    private IPage? _page;
    
    public static Playwright Instance { get; } = new();
    
    public IPage? Page => _page;

    /// <summary>
    /// Opens a new browser
    /// </summary>
    /// <param name="browserType">Type of the browser to open</param>
    /// <returns>The active page of the launched browser</returns>
    public async Task<IPage> OpenBrowser(string browserType = "Chromium")
    {
        var playwright = await Microsoft.Playwright.Playwright.CreateAsync();

        var browser = browserType switch
        {
            "Firefox" => await playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false }),
            _ => await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false })
        };

        _browser = browser;
        _page = await _browser.NewPageAsync();
        return _page;
    }

    /// <summary>
    /// Closes the browser
    /// </summary>
    public async Task CloseBrowser()
    {
        ExtentReportHolder.LogMessage("Closing the browser...");
        if (_page is null) return;

        await _page.CloseAsync();
        _page = null;
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
        await _page!.WaitForLoadStateAsync(LoadState.Load,
            new PageWaitForLoadStateOptions { Timeout = timeoutInSeconds * 1000 });
        var elements = await _page!.QuerySelectorAllAsync(selector);
        return elements.ToList();
    }

    /// <summary>
    /// Types to an element on the page
    /// </summary>
    /// <param name="selector">The selector of the element to type into</param>
    /// <param name="text">The text typed to the element</param>
    public async Task TypeToElement(string selector, string? text)
    {
        var element = await _page!.WaitForSelectorAsync(selector);
        if (element is not null) await element.FillAsync(text!);
    }

    /// <summary>
    /// Clicks on a specified element
    /// </summary>
    /// <param name="selector">The selector of the element to click</param>
    public async Task ClickOnElement(string selector)
    {
        var element = await _page!.WaitForSelectorAsync(selector);
        if (element is not null) await element.ClickAsync();
    }
    
    /// <summary>
    /// Retrieves the text content from an element
    /// </summary>
    /// <param name="element">The element to retrieve text from</param>
    /// <returns>The text of element</returns>
    public async Task<string?> GetElementText(IElementHandle element) => await element.InnerTextAsync();

    /// <summary>
    /// Reloads the current page
    /// </summary>
    public async Task RefreshPage() => await _page!.ReloadAsync();
}
