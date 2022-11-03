using BlazorCleanArchitecture.Domain.Tenant;
using BlazorCleanArchitecture.Domain.User;
using BlazorCleanArchitecture.Infrastructure.Data;
using BlazorCleanArchitecture.Shared.Authentication.Commands;
using BlazorCleanArchitecture.Shared.Common.Exceptions;
using FluentAssertions;
using Google.Authenticator;
using System.IdentityModel.Tokens.Jwt;
using Xunit;

namespace BlazorCleanArchitecture.Application.IntegrationTests.Authentication.Commands
{
    public sealed class LoginTests : IClassFixture<TestFixture>, IAsyncLifetime
    {
        private readonly TestFixture _testing;

        private Domain.User.User User = new Domain.User.User
        {
            FirstName = "Test",
            LastName = "Tester",
            Username = "test@test.com",
            Email = "test@test.com",
            Password = "Abcdefgh1!",
            MFAKey = Guid.NewGuid()
        };

        public LoginTests(TestFixture testing)
            => _testing = testing;

        internal async Task Setup()
        {
            var tenant = new Domain.Tenant.Tenant
            {
                Name = "Test",
                Domain = _testing.CurrentUser.Username.Split("@")[1]
            };

            await _testing.AddAsync<ApplicationDbContext, Tenant>(tenant);

            User.TenantId = tenant.Id;

            await _testing.AddAsync<ApplicationDbContext, Domain.User.User>(User);
        }

        internal async Task Cleanup()
        {
            await _testing.ClearAsync<ApplicationDbContext, PasswordReset>();
            await _testing.ClearAsync<ApplicationDbContext, Domain.User.User>();
        }

        public Task DisposeAsync()
        {
            return Cleanup();
        }

        public Task InitializeAsync()
        {
            return Setup();
        }

        [Fact]
        public async Task LoginSuccessfully()
        {
            var result = await _testing.SendAsync(new Login
            {
                Username = "test@test.com",
                Password = "Abcdefgh1!",
                MFACode = int.Parse(new TwoFactorAuthenticator().GetCurrentPIN(User.MFAKey.ToString()))
            });

            result.Should().NotBeNull();

            var token = new JwtSecurityTokenHandler().ReadJwtToken(result);

            token.Issuer.Should().Be("BlazorCleanArchitecture");
            token.Audiences.Should().Contain("BlazorCleanArchitecture");
            token.Subject.Should().Be("BlazorCleanArchitecture");

            token.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value.Should().Be("test@test.com");
        }

        [Fact]
        public async Task UserDoesntExist()
        {
            var act = async () => await _testing.SendAsync(new Login { Username = "abc@def.xyz", Password = "IncorrectPassword1!", MFACode = int.Parse(new TwoFactorAuthenticator().GetCurrentPIN(User.MFAKey.ToString())) });

            var result = await act.Should().ThrowAsync<ValidationException>();

            result.Which.Errors.Should().BeEquivalentTo(new Dictionary<string, string[]>
            {
                { "Username", new[] { "User does not exist." } }
            });
        }

        [Theory]
        [InlineData(null, "'Username' must not be empty.")]
        [InlineData("abc", "'Username' is not a valid email address.")]
        [InlineData("testtesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttest@test.com", "The length must be no more than 320 characters.")]
        public async Task IncorrectUsername(string username, string expectedOutcome)
        {
            var act = async () => await _testing.SendAsync(new Login { Username = username });

            var result = await act.Should().ThrowAsync<ValidationException>();

            result.Which.Errors.Should().BeEquivalentTo(new Dictionary<string, string[]>
            {
                { "Username", new[] { expectedOutcome } }
            });
        }

        [Theory]
        [InlineData(null, "'Password' must not be empty.")]
        [InlineData("a", "The length must be at least 8 characters.")]
        [InlineData("aaaaaaaa", "Password must contain atleast 1 number.")]
        [InlineData("Aaaaaaaa1", "Password must contain atleast 1 special character.")]
        [InlineData("aaaaaaaa1!", "Password must contain atleast 1 upper-case character.")]
        [InlineData("AAAAAAAA1!", "Password must contain atleast 1 lower-case character.")]
        [InlineData("Aaaaaaaa1! ", "Password must not contain any white spaces.")]
        public async Task IncorrectPassword(string password, string expectedOutcome)
        {
            var act = async () => await _testing.SendAsync(new Login { Username = "test@test.com", Password = password });

            var result = await act.Should().ThrowAsync<ValidationException>();

            result.Which.Errors.Should().BeEquivalentTo(new Dictionary<string, string[]>
            {
                { "Password", new[] { expectedOutcome } }
            });
        }

        [Fact]
        public async Task AttemptLoginWithResetPasswordOutstanding()
        {
            await _testing.SendAsync(new ForgotPassword { Username = "test@test.com" });

            var act = async () => await _testing.SendAsync(new Login { Username = "test@test.com", Password = "Abcdefgh1!", MFACode = int.Parse(new TwoFactorAuthenticator().GetCurrentPIN(User.MFAKey.ToString())) });

            var result = await act.Should().ThrowAsync<ValidationException>();

            result.Which.Errors.Should().BeEquivalentTo(new Dictionary<string, string[]>
            {
                { "Username", new[] { "Password reset is currently outstanding, please reset your password before attempting to login." } }
            });
        }

        [Fact]
        public async Task LoginWithInvalidMFA()
        {
            var act = async () => await _testing.SendAsync(new Login { Username = "test@test.com", Password = "Abcdefgh1!", MFACode = 123456 });

            var result = await act.Should().ThrowAsync<ValidationException>();

            result.Which.Errors.Should().BeEquivalentTo(new Dictionary<string, string[]>
            {
                { "Multi-factor authentication", new[] { "Failed to validate multi-factor authentication." } }
            });
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        [InlineData(1000000)]
        public async Task MFACodeInvalid(int code)
        {
            var act = async () => await _testing.SendAsync(new Login { Username = "test@test.com", Password = "Abcdefgh1!", MFACode = code });

            var result = await act.Should().ThrowAsync<ValidationException>();

            result.Which.Errors.Should().BeEquivalentTo(new Dictionary<string, string[]>
            {
                { "MFACode", new[] { "The length must be exactly 6 characters." } }
            });
        }
    }
}
