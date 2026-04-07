using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace Frontend_tests.Infrastructure;

[Parallelizable(ParallelScope.Self)] // Omogućava paralelno pokretanje UI testova
public class UiTestBase : PageTest
{
    // Ovde možeš dodati zajedničke metode za sve UI testove (npr. Login)
}