using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Config;
using Microsoft.Playwright;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace TestAssignment.Resources.Utilities;

public static class ExtentReportHolder
{
    private const string ReportDirectory = @"C:\MES\AirBnbAutomationResults";
    private static ExtentReports? _extent;
    private static ExtentTest? _currentTest;
    private static string? _reportPath;

    public static void InitializeReport()
    {
        var suiteName = TestContext.CurrentContext.Test.DisplayName;
        var timestamp = DateTime.Now.ToString("yyyyMMdd");
        var reportFileName = $"{suiteName}_TestReport_{timestamp}.html";

        _reportPath = Path.Combine(ReportDirectory, reportFileName);

        if (!Directory.Exists(ReportDirectory)) Directory.CreateDirectory(ReportDirectory);
        if (File.Exists(_reportPath)) File.Delete(_reportPath);

        var sparkReporter = new ExtentSparkReporter(_reportPath)
        {
            Config =
            {
                Theme = Theme.Dark,
                ReportName = $"{suiteName} Automation Test Run"
            }
        };
        _extent = new ExtentReports();
        _extent.AttachReporter(sparkReporter);
    }

    public static void SetCurrentTest(ExtentTest? test) => _currentTest = test;

    public static void LogMessage(string message)
    {
        Console.WriteLine(message);
        _currentTest?.Info(message);
    }

    public static ExtentTest? CreateTest(string testName)
    {
        var test = _extent!.CreateTest(testName);
        LogTestDescription(test);

        return test;
    }

    private static void LogTestDescription(ExtentTest? test)
    {
        var description = TestContext.CurrentContext.Test.Properties.Get("Description") as string;

        if (!string.IsNullOrEmpty(description)) test?.Info($"Description: {description}");
    }

    public static void FlushReport() => _extent!.Flush();

    public static void LogTestResult(ExtentTest? test, TestStatus status, string testName, string message = "", string? stackTrace = "")
    {
        var screenshotPath = TakeScreenshot(testName);

        _ = status switch
        {
            TestStatus.Failed => test?.Fail(message).Fail(stackTrace)
                .Fail(MediaEntityBuilder.CreateScreenCaptureFromPath(screenshotPath).Build()),
            
            TestStatus.Passed => test?.Pass(message)
                .Pass(MediaEntityBuilder.CreateScreenCaptureFromPath(screenshotPath).Build()),
            
            TestStatus.Skipped => test?.Skip("Test skipped")
                .Skip(MediaEntityBuilder.CreateScreenCaptureFromPath(screenshotPath).Build()),
            
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }
    
    private static string TakeScreenshot(string testName)
    {
        var page = Playwright.Instance.Page;
        if (page is null) throw new InvalidOperationException("Browser page is not initialized.");

        var screenshotDirectory = Path.Combine(ReportDirectory, "Screenshots");
        if (!Directory.Exists(screenshotDirectory)) Directory.CreateDirectory(screenshotDirectory);

        var screenshotFilePath = Path.Combine(screenshotDirectory, $"{testName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");

        page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotFilePath });
        return screenshotFilePath;
    }
}