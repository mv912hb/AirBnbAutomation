using NUnit.Framework;
using TestAssignment.Resources;
using TestAssignment.Resources.FilesForTests;
using TestAssignment.Resources.Pages;

namespace TestAssignment.Tests;

[TestFixture]
public class AirbnbTests : BaseClass
{
    [Test, TestCaseSource(typeof(TestCaseSources), nameof(TestCaseSources.GetTestCases))]
    public async Task AirbnbTest(string browserType, string city, string dateFrom, string dateTo, int price, int cleaningFee)
    {
        await Playwright.Instance.OpenBrowser(browserType);
        await MainPage.Instance.SearchForApartments(city, dateFrom, dateTo, 2, 2);
        await SearchResultsPage.Instance.FindApartmentUnderPrice(price);
        Assert.That(await SearchResultsPage.Instance.GetCleaningFee(),
            Is.LessThanOrEqualTo(cleaningFee), "Cleaning fee is higher than expected");
    }
}