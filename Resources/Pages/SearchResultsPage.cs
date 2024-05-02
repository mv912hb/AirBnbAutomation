namespace TestAssignment.Resources.Pages;

public class SearchResultsPage
{
    private static readonly string ResultItem = "xpath=//div[@data-testid='card-container']";
    
    public static SearchResultsPage Instance { get; } = new();
    
    public async Task<List<string>> GetAllPageResults()
    {
        var elements = await Playwright.Instance.FindElements(ResultItem);
        var tasks = elements.Select(element => element.InnerTextAsync()).ToList();
        var results = await Task.WhenAll(tasks);
        return results.ToList();
    }
}