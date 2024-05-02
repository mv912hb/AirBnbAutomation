using Microsoft.Playwright;
using TestAssignment.Resources.Utilities;

namespace TestAssignment.Resources;

public class Playwright
{
    private IBrowser? _browser;
    private IPage? _page;

    public static Playwright Instance { get; } = new();

    public IPage? Page
    {
        get
        {
            if (_page is null) OpenBrowser().Wait();
            return _page;
        }
    }

    public async Task OpenBrowser()
    {
        ExtentReportHolder.LogMessage("Opening the browser...");
        if (_page is not null) return;

        var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        _browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
            Args = new[] { "--start-fullscreen" }
        });
        _page = await _browser.NewPageAsync();
    }

    public async Task CloseBrowser()
    {
        ExtentReportHolder.LogMessage("Closing the browser...");
        if (_page == null) return;
        await _page.CloseAsync();
        _page = null;
        await _browser!.CloseAsync();
    }

    public async Task<IElementHandle?> FindElement(string selector, int timeoutInSeconds = 10)
    {
        if (_page is null) await OpenBrowser();
        return await _page!.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions { Timeout = timeoutInSeconds * 1000 });
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
    
    public async Task<IEnumerable<IElementHandle>> FindElements(string selector, int timeoutInSeconds = 10)
    {
        if (_page == null) throw new InvalidOperationException("The browser page is not initialized.");
        await _page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions { Timeout = timeoutInSeconds * 1000 });
        return await _page.QuerySelectorAllAsync(selector);
    }

    public async Task<bool> IsElementDisplayed(string selector)
    {
        try
        {
            var element = await FindElement(selector);
            return element != null && await element.IsVisibleAsync();
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task NavigateToUrl(string url)
    {
        if (_page == null) await OpenBrowser();
        await _page.GotoAsync(url);
    }

    public async Task RestartBrowser()
    {
        await CloseBrowser();
        await Task.Delay(2000); // Wait for 2 seconds
        await OpenBrowser();
    }

    public async Task RefreshPage() => await _page?.ReloadAsync();

    public async Task ScrollToTopOfPage()
    {
        if (_page == null) throw new InvalidOperationException("The browser page is not initialized.");
        await _page.EvaluateAsync("window.scrollTo(0, 0);");
    }
}