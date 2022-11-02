using BlazorCleanArchitecture.WebUI.GrpcTests.Helpers;
using Xunit.Abstractions;

namespace BlazorCleanArchitecture.WebUI.GrpcTests.Authentication.Commands
{
    public sealed class LoginTests : TestBase
    {
        public LoginTests(GrpcTestFixture<Program> fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
        {

        }
    }
}
