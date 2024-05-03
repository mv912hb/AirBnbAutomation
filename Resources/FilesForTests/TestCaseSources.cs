using NUnit.Framework;

namespace TestAssignment.Resources.FilesForTests;

public static class TestCaseSources
{
    private static readonly string[] Cities = ["Tel-Aviv", "London"];
    private static readonly string[] DatesFrom = ["05/14/2024", "05/15/2024"];
    private static readonly string[] DatesTo = ["05/19/2024", "05/24/2024"];
    private static readonly string[] BrowserTypes = ["Chromium", "Firefox"]; 
    private static readonly int[] Prices = [1000, 500];
    private static readonly int[] CleaningFees = [100, 200];
        
    public static IEnumerable<TestCaseData> GetTestCases() =>
        from browserType in BrowserTypes
        from city in Cities
        from dateFrom in DatesFrom
        from dateTo in DatesTo
        from price in Prices
        from cleaningFee in CleaningFees
        select new TestCaseData(browserType, city, dateFrom, dateTo, price, cleaningFee);
}