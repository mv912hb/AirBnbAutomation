using AventStack.ExtentReports;
using NUnit.Framework;
using TestAssignment.Resources;
using TestAssignment.Resources.Utilities;

namespace TestAssignment.Tests;

public class BaseClass
{
    private ExtentTest? _test;

    [OneTimeSetUp]
    public void BeforeSuite() => ExtentReportHolder.InitializeReport();

    [SetUp]
    public void BeforeMethod()
    {
        var testName = TestContext.CurrentContext.Test.Name;
        _test = ExtentReportHolder.CreateTest(testName);
        ExtentReportHolder.SetCurrentTest(_test);
    }

    [TearDown]
    public void AfterMethod()
    {
        var status = TestContext.CurrentContext.Result.Outcome.Status;
        var stackTrace = TestContext.CurrentContext.Result.StackTrace;
        var errorMessage = TestContext.CurrentContext.Result.Message;

        ExtentReportHolder.LogTestResult(_test, status, TestContext.CurrentContext.Test.Name, errorMessage, stackTrace);
        ExtentReportHolder.FlushReport();
    }

    [OneTimeTearDown]
    public async Task AfterSuite()
    {
        await Playwright.Instance.CloseBrowser();
    }
}