using Microsoft.Playwright;
using TestAssignment.Resources.Utilities;

namespace TestAssignment.Resources;

public class Playwright
{
    private IBrowser? _browser;
    private static IPage? Page { get; set; }

    public static Playwright Instance { get; } = new();

    public static IPage? GetPage() => Page;

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
    }

    public async Task CloseBrowser()
    {
        ExtentReportHolder.LogMessage("Closing the browser...");
        if (Page is null) return;

        await Page.CloseAsync();
        Page = null;
        if (_browser is not null) await _browser.DisposeAsync();
    }

    public async Task<List<IElementHandle>> FindElements(string selector, int timeoutInSeconds = 10)
    {
        await Page!.WaitForLoadStateAsync(LoadState.NetworkIdle,
            new PageWaitForLoadStateOptions { Timeout = timeoutInSeconds * 1000 });
        var elements = await Page!.QuerySelectorAllAsync(selector);
        return elements.ToList();
    }

    public async Task TypeToElement(string selector, string? text)
    {
        var element = await Page!.WaitForSelectorAsync(selector);
        if (element is not null) await element.FillAsync(text!);
    }

    public async Task ClickOnElement(string selector)
    {
        var element = await Page!.WaitForSelectorAsync(selector);
        if (element is not null) await element.ClickAsync();
    }

    public async Task<string?> GetElementText(IElementHandle element) => await element.InnerTextAsync();

    public async Task RefreshPage() => await Page!.ReloadAsync();
}