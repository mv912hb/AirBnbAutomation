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
            Headless = false
        }).GetAwaiter().GetResult();
        
        _page = _browser.NewPageAsync().GetAwaiter().GetResult();
        _page.EvaluateAsync("window.moveTo(0,0); window.resizeTo(screen.width, screen.height);").GetAwaiter().GetResult();
    }

    public void CloseBrowser()
    {
        ExtentReportHolder.LogMessage("Closing the browser...");
        if (_page is null) return;
        _page.CloseAsync();
        _page = null;
        _browser!.CloseAsync();
    }

    private async Task<IElementHandle?> FindElement(string selector, int timeoutInSeconds = 10)
    {
        await _page!.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions { Timeout = timeoutInSeconds * 1000 });
        return await _page!.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions { Timeout = timeoutInSeconds * 1000 });
    }

    public async Task<List<IElementHandle>> FindElements(string selector, int timeoutInSeconds = 10)
    {
        await _page!.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions { Timeout = timeoutInSeconds * 1000 });
        
        var elements = await _page!.QuerySelectorAllAsync(selector);
        return elements.ToList();
    }
    
    public async Task TypeToElement(string selector, string text)
    {
        var element = await FindElement(selector);
        if (element is not null) await element.FillAsync(text);
    }
    
    public async Task ClickOnElement(string selector)
    {
        await _page!.WaitForLoadStateAsync(LoadState.Load);
        var element = await FindElement(selector);
        if (element is not null) await element.ClickAsync();
    }

    public async Task<string?> GetElementText(IElementHandle element) => await element.InnerTextAsync();
    
    public async Task RestartBrowser()
    {
        CloseBrowser();
        await Task.Delay(2000);
        OpenBrowser();
    }

    public async Task RefreshPage() => await _page!.ReloadAsync();
}