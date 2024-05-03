using NUnit.Framework;
using TestAssignment.Resources;
using TestAssignment.Resources.FilesForTests;
using TestAssignment.Resources.Pages;

namespace TestAssignment.Tests;

[TestFixture]
public class AirbnbTests : BaseClass
{
    //[Test, TestCaseSource(typeof(TestCaseSources), nameof(TestCaseSources.GetTestCases))]
    //public void AirbnbTest(string browserType, string city, string dateFrom, string dateTo, int price, int cleaningFee)
    [Test]
    public void AirbnbTest()
    {
        MainPage.Instance
            .Navigate()
            .ChooseDestination("Tel-Aviv")
            .ChooseDates("05/14/2024", "05/18/2024")
            .AddGuests(1, 0)
            .Search();
        
        SearchResultsPage.Instance.FindApartmentUnderPrice(1000);

        Console.WriteLine(SearchResultsPage.Instance.GetCleaningFee().ToString());
        
        Assert.That(SearchResultsPage.Instance.GetCleaningFee(),
            Is.LessThanOrEqualTo(200), "Cleaning fee is higher than expected");
    }
}