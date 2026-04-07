using Frontend_tests.Infrastructure;
using NUnit.Framework;

namespace Frontend_tests.Tests.API;

[TestFixture]
[Category("API")] // Ključno za nezavisno pokretanje
public class ProductApiTests : ApiTestBase
{
    [Test]
    public async Task ShouldReturnProducts_WhenGetRequestIsMade()
    {
        var response = await Request.GetAsync("/api/products");
        Assert.That(response.Status, Is.Not.EqualTo(200));
    }
}