using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;

namespace FrontendPlaywright.Tests
{
    [TestFixture]
    public class E2ETests
    {
        private IPlaywright _playwright;
        private IBrowser _browser;

        [OneTimeSetUp]
        public async Task GlobalSetup()
        {
            _playwright = await Playwright.CreateAsync();
            try
            {
                await Playwright.InstallAsync();
            }
            catch
            {
                // If install isn't available in environment, tests will try to run with already-installed browsers
            }

            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        }

        [Test]
        public async Task HomePage_Loads_and_has_title()
        {
            var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL") ?? "http://localhost:8080";
            await using var context = await _browser.NewContextAsync();
            var page = await context.NewPageAsync();
            var response = await page.GotoAsync(frontendUrl);
            Assert.IsTrue(response.Ok, "Front-end did not return a successful response");
            var title = await page.TitleAsync();
            Assert.IsNotNull(title);
        }

        [OneTimeTearDown]
        public async Task GlobalTeardown()
        {
            if (_browser != null) await _browser.CloseAsync();
            _playwright?.Dispose();
        }
    }
}
