using NUnit.Framework;
using TestAssignment.Resources.Pages;

namespace TestAssignment.Tests;

[TestFixture]
public class AirbnbTests : BaseClass
{
    [Test]
    public async Task AirbnbTest()
    {
        await MainPage.Instance.SearchForApartments("Tel-Aviv", "05/14/2024", "05/19/2024",3, 3);
        await SearchResultsPage.Instance.FindApartmentUnderPrice(1000);
    }
}