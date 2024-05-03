using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace TestAssignment.Resources;

public class Selenium
{
    private IWebDriver? _driver;

    public static Selenium Instance { get; } = new();

    public IWebDriver? Driver
    {
        get
        {
            if (_driver is null) OpenBrowser();
            return _driver;
        }
    }

    public void OpenBrowser()
    {
        if (_driver is not null) return;
        var options = new ChromeOptions();

        //options.AddArgument("--headless");
        options.AddArgument("--disable-gpu");
        options.AddArgument("--start-fullscreen");

        _driver = new ChromeDriver(options);
    }

    public void CloseBrowser()
    {
        if (_driver is null) return;
        _driver.Quit();
        _driver = null;
    }

    public IWebElement FindElement(By by, int timeoutInSeconds = 10, double pollingIntervalInSeconds = 0.5)
    {
        if (_driver is null) OpenBrowser();
        
        var pageLoadWait = new WebDriverWait(_driver, TimeSpan.FromSeconds(timeoutInSeconds));
        pageLoadWait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
    
        var wait = CreateDefaultWait(timeoutInSeconds, pollingIntervalInSeconds);
        try
        {
            return TryFindElement(wait, by);
        }
        catch (StaleElementReferenceException)
        {
            return TryFindElement(wait, by);
        }
        catch (NoSuchElementException)
        {
            throw new NoSuchElementException($"Element by {by} could not be found within the timeout period.");
        }
    }
    
    public void TypeToElement(By by, string text, int timeoutInSeconds = 10, double pollingIntervalInSeconds = 0.5) => 
        FindElement(by, timeoutInSeconds, pollingIntervalInSeconds).SendKeys(text);
    
    public void ClickOnElement(By by, int timeoutInSeconds = 10, double pollingIntervalInSeconds = 0.5) => 
        FindElement(by, timeoutInSeconds, pollingIntervalInSeconds).Click();
    
    private DefaultWait<IWebDriver> CreateDefaultWait(int timeoutInSeconds, double pollingIntervalInSeconds)
    {
        var wait = new DefaultWait<IWebDriver>(_driver ?? throw new InvalidOperationException())
        {
            Timeout = TimeSpan.FromSeconds(timeoutInSeconds),
            PollingInterval = TimeSpan.FromSeconds(pollingIntervalInSeconds)
        };
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
        return wait;
    }

    private static IWebElement TryFindElement(DefaultWait<IWebDriver> wait, By by) => 
        wait.Until(drv =>
        {
            var foundElement = drv.FindElement(by);
            if (foundElement.Displayed) return foundElement;
            throw new NoSuchElementException($"Element by {by} was not displayed.");
        });

    public IEnumerable<IWebElement> FindElements(By by, int timeoutInSeconds = 10,
        double pollingIntervalInSeconds = 0.5)
    {
        var pageLoadWait = new WebDriverWait(_driver, TimeSpan.FromSeconds(timeoutInSeconds));
        pageLoadWait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
        
        if (_driver is null) throw new InvalidOperationException("The WebDriver is not initialized.");
        var wait = CreateDefaultWait(timeoutInSeconds, pollingIntervalInSeconds);

        wait.IgnoreExceptionTypes(typeof(NoSuchElementException));

        try
        {
            return wait.Until(drv =>
            {
                var elements = drv.FindElements(by);
                if (elements.Any(e => e.Displayed)) return elements;
                throw new NoSuchElementException($"None of the elements by {by} were displayed.");
            });
        }
        catch (WebDriverTimeoutException)
        {
            throw new NoSuchElementException(
                $"Elements by {by} could not be found or were not visible within the timeout period.");
        }
    }
    
    public string GetElementText(IWebElement element) => element.Text;

    public bool IsElementDisplayed(By element)
    {
        try { return FindElement(element).Displayed; }
        catch (NoSuchElementException) { return false; }
    }

    public void NavigateToUrl(string url)
    {
        if (_driver is null) OpenBrowser();
        _driver?.Navigate().GoToUrl(url);
    }

    public void RestartBrowser()
    {
        CloseBrowser();
        Thread.Sleep(2000);
        OpenBrowser();
    }

    public void RefreshPage() => _driver?.Navigate().Refresh(); 

    public void ScrollToTopOfPage()
    {
        if (_driver is null) throw new InvalidOperationException("The WebDriver is not initialized.");

        var js = (IJavaScriptExecutor)_driver;
        js.ExecuteScript("window.scrollTo(0, 0);");
    }
}