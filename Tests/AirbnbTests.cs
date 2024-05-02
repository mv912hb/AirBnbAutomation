using NUnit.Framework;
using TestAssignment.Resources.Pages;

namespace TestAssignment.Tests;

[TestFixture]
public class AirbnbTests : BaseClass
{
    [Test]
    public async Task TestNavigateAndTypeDestination()
    {
        await MainPage.Instance
            .Navigate()
            .ContinueWith(async page => await page.Result.ChooseDestination("Tel-Aviv"));
    }
}