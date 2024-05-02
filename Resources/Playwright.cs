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
            if (_page is null) OpenBrowser();
            return _page;
        }
    }

    public void OpenBrowser()
    {
        ExtentReportHolder.LogMessage("Opening the browser...");
        if (_page is not null) return;

        var playwright = Microsoft.Playwright.Playwright.CreateAsync().GetAwaiter().GetResult();
        _browser = playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
            Args = new[] { "--start-fullscreen" }
        }).GetAwaiter().GetResult();
        _page = _browser.NewPageAsync().GetAwaiter().GetResult();
    }

    public void CloseBrowser()
    {
        ExtentReportHolder.LogMessage("Closing the browser...");
        if (_page is null) return;
        _page.CloseAsync().GetAwaiter().GetResult();
        _page = null;
        _browser!.CloseAsync().GetAwaiter().GetResult();
    }

    public async Task<IElementHandle?> FindElement(string selector, int timeoutInSeconds = 10)
    {
        if (_page is null) OpenBrowser();
        return await _page!.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions { Timeout = timeoutInSeconds * 1000 });
    }
    
    public void TypeToElement(string selector, string text) => FindElement(selector).GetAwaiter().GetResult()?.FillAsync(text);
    
    public void ClickOnElement(string selector) => FindElement(selector).GetAwaiter().GetResult()?.ClickAsync();
    
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

    public void NavigateToUrl(string url)
    {
        if (_page == null) OpenBrowser();
        _page?.GotoAsync(url).GetAwaiter().GetResult();
    }

    public async Task RestartBrowser()
    {
        CloseBrowser();
        await Task.Delay(2000); // Wait for 2 seconds
        OpenBrowser();
    }

    public async Task RefreshPage() => await _page?.ReloadAsync();

    public async Task ScrollToTopOfPage()
    {
        if (_page == null) throw new InvalidOperationException("The browser page is not initialized.");
        await _page.EvaluateAsync("window.scrollTo(0, 0);");
    }
}