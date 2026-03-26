using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;

namespace FrontendPlaywright.Tests
{
    [TestFixture]
    public class ApiTests
    {
        private IPlaywright _playwright;
        private APIRequestContext _request;

        [OneTimeSetUp]
        public async Task GlobalSetup()
        {
            _playwright = await Playwright.CreateAsync();
            var apiBase = Environment.GetEnvironmentVariable("API_URL") ?? "http://localhost:5000";
            _request = await _playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
            {
                BaseURL = apiBase
            });
        }

        [Test]
        public async Task Get_Venues_Returns_OK()
        {
            var response = await _request.GetAsync("/api/venues");
            Assert.IsTrue(response.Ok, "API did not return OK for /api/venues");
        }

        [OneTimeTearDown]
        public async Task GlobalTeardown()
        {
            if (_request != null) await _request.DisposeAsync();
            _playwright?.Dispose();
        }
    }
}
