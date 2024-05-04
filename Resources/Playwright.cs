using Microsoft.Playwright;
using TestAssignment.Resources.Utilities;

namespace TestAssignment.Resources;

public class Playwright
{
    private IBrowser? _browser;
    private IPage? _page;
    
    public static Playwright Instance { get; } = new();
    
    public IPage? Page => _page;

    public async Task OpenBrowser(string browserType = "Chromium")
    {
        var playwright = await Microsoft.Playwright.Playwright.CreateAsync();

        var browser = browserType switch
        {
            "Firefox" => await playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false }),
            _ => await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false })
        };

        _browser = browser;
        _page = await _browser.NewPageAsync();
    }

    public async Task CloseBrowser()
    {
        ExtentReportHolder.LogMessage("Closing the browser...");
        if (_page is null) return;

        await _page.CloseAsync();
        _page = null;
        if (_browser is not null) await _browser.CloseAsync();
    }

    public async Task<List<IElementHandle>> FindElements(string selector, int timeoutInSeconds = 10)
    {
        await _page!.WaitForLoadStateAsync(LoadState.NetworkIdle,
            new PageWaitForLoadStateOptions { Timeout = timeoutInSeconds * 1000 });
        var elements = await _page!.QuerySelectorAllAsync(selector);
        return elements.ToList();
    }

    public async Task TypeToElement(string selector, string? text)
    {
        var element = await _page!.WaitForSelectorAsync(selector);
        if (element is not null) await element.FillAsync(text!);
    }

    public async Task ClickOnElement(string selector)
    {
        var element = await _page!.WaitForSelectorAsync(selector);
        if (element is not null) await element.ClickAsync();
    }

    public async Task ScrollPageUp() => await _page!.Mouse.UpAsync();
    
    public async Task<string?> GetElementText(IElementHandle element) => await element.InnerTextAsync();

    public async Task RefreshPage() => await _page!.ReloadAsync();
}
