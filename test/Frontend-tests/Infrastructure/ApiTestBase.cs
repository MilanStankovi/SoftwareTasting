using Microsoft.Playwright;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Frontend_tests.Infrastructure;

public class ApiTestBase
{
    protected IAPIRequestContext Request = null!;
    private IPlaywright _playwright = null!;

    [SetUp]
    public async Task ApiSetup()
    {
        _playwright = await Playwright.CreateAsync();
        
        // Promenjeno sa APIRequestSourceOptions na APIRequestNewContextOptions
        Request = await _playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
        {
            // Ovde stavi URL tvog API-ja (npr. tvog Shopora bekenda)
            BaseURL = "http://localhost:5227",
            IgnoreHTTPSErrors = true // Dobra praksa za lokalni razvoj
        });
    }

    [TearDown]
    public async Task ApiTearDown()
    {
        if (Request != null) await Request.DisposeAsync();
        _playwright?.Dispose();
    }
}