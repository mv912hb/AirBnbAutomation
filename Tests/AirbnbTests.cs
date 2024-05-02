using NUnit.Framework;
using TestAssignment.Resources.Pages;

namespace TestAssignment.Tests;

[TestFixture]
public class AirbnbTests : BaseClass
{
    [Test]
    public async Task AirbnbTest()
    {
        await MainPage.Instance.SearchForApartments("Tel-Aviv", "05/14/2024", "05/19/2024",2, 2);
        var results = await SearchResultsPage.Instance.GetAllPageResults();
        
        foreach (var text in results)
        {
            Console.WriteLine(text);
        }
    }
}