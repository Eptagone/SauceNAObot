// Here you could define global logic that would affect all tests

// You can use attributes at the assembly level to apply to all tests in the assembly
[assembly: Retry(3)]
[assembly: System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]

namespace SauceNao.Tests;

public class GlobalHooks
{
    [Before(TestSession)]
    public static void SetUp() { }

    [After(TestSession)]
    public static void CleanUp() { }
}
