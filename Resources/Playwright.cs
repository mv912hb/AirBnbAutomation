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

    public IElementHandle? FindElement(string selector, int timeoutInSeconds = 10)
    {
        if (_page is null) OpenBrowser();
        return _page!
            .WaitForSelectorAsync(selector, new PageWaitForSelectorOptions { Timeout = timeoutInSeconds * 1000 })
            .GetAwaiter()
            .GetResult();
    }
    
    public void TypeToElement(string selector, string text) => FindElement(selector)?.FillAsync(text);
    
    public void ClickOnElement(string selector) => FindElement(selector)?.ClickAsync();

    public async Task RestartBrowser()
    {
        CloseBrowser();
        await Task.Delay(2000);
        OpenBrowser();
    }

    public void RefreshPage() => _page?.ReloadAsync();

    public void ScrollToTopOfPage()
    {
        if (_page == null) throw new InvalidOperationException("The browser page is not initialized.");
        _page.EvaluateAsync("window.scrollTo(0, 0);");
    }
}