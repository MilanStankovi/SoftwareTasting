using Frontend_tests.Infrastructure;
using NUnit.Framework;

namespace Frontend_tests.Tests.UI;

[TestFixture]
[Category("UI")] // Ključno za nezavisno pokretanje
public class LoginUiTests : UiTestBase
{
    [Test]
    public async Task ShouldShowWelcomeMessage_AfterLogin()
    {
        await Page.GotoAsync("http://localhost:5227/login"); // tvoj React/Frontend
        await Page.FillAsync("input[name='email']", "milan@example.com");
        await Page.ClickAsync("button[type='submit']");
        
        await Expect(Page.Locator(".welcome-msg")).ToBeVisibleAsync();
    }
}