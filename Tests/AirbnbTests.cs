using NUnit.Framework;
using TestAssignment.Resources.Pages;

namespace TestAssignment.Tests;

[TestFixture]
public class AirbnbTests : BaseClass
{
    [Test]
    public void AirbnbTest()
    {
        MainPage.Instance
            .ChooseDestination("Tel-Aviv")
            .ChooseDates("05/14/2024", "05/19/2024");
    }
}