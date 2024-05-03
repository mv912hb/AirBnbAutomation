using Microsoft.Playwright;
using TestAssignment.Resources.Utilities;

namespace TestAssignment.Resources;

public class Playwright
{
    private IBrowser? _browser;

    public static Playwright Instance { get; } = new();

    public IPage? Page { get; private set; }

    public async Task OpenBrowser()
    {
        ExtentReportHolder.LogMessage("Opening the browser...");
        if (Page is not null) return;

        var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        _browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false
        });

        Page = await _browser.NewPageAsync();
    }

    public async Task CloseBrowser()
    {
        ExtentReportHolder.LogMessage("Closing the browser...");
        if (Page is null) return;

        await Page.CloseAsync();
        Page = null;
        if (_browser is not null) await _browser.CloseAsync();
    }

    public async Task<IElementHandle?> FindElement(string selector, int timeoutInSeconds = 10)
    {
        if (Page is null) throw new InvalidOperationException("The page object is not initialized.");

        try
        {
            var options = new PageWaitForSelectorOptions
            {
                Timeout = timeoutInSeconds * 1000
            };
            var element = await Page.WaitForSelectorAsync(selector, options);
            return element;
        }
        catch (TimeoutException)
        {
            throw new Exception($"Element with selector '{selector}' could not be found within the timeout period.");
        }
    }

    public async Task<List<IElementHandle>> FindElements(string selector, int timeoutInSeconds = 10)
    {
        await Page!.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions { Timeout = timeoutInSeconds * 1000 });
        var elements = await Page!.QuerySelectorAllAsync(selector);
        return elements.ToList();
    }
    
    public async Task TypeToElement(string selector, string text)
    {
        var element = await FindElement(selector);
        if (element is not null) await element.FillAsync(text);
    }
    
    public async Task ClickOnElement(string selector)
    {
        var element = await FindElement(selector);
        if (element is not null) await element.ClickAsync();
    }

    public async Task<string?> GetElementText(IElementHandle element) => await element.InnerTextAsync();
    
    public async Task RestartBrowser()
    {
        await CloseBrowser();
        await Task.Delay(2000);
        await OpenBrowser();
    }

    public async Task RefreshPage() => await Page!.ReloadAsync();
}