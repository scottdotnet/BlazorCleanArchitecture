using BlazorCleanArchitecture.Infrastructure.Data;
using BlazorCleanArchitecture.Shared.Authentication.Commands;
using BlazorCleanArchitecture.Shared.Common.Exceptions;
using FluentAssertions;
using System.IdentityModel.Tokens.Jwt;
using Xunit;

namespace BlazorCleanArchitecture.Application.IntegrationTests.Authentication.Commands
{
    public sealed class LoginTests : IClassFixture<TestFixture>, IAsyncLifetime
    {
        private readonly TestFixture _testing;

        public LoginTests(TestFixture testing)
            => _testing = testing;

        internal async Task Setup()
        {
            var tenant = new Domain.Tenant.Tenant
            {
                Name = "Test",
                Domain = _testing.CurrentUser.Username.Split("@")[1]
            };

            await _testing.AddAsync<ApplicationDbContext, Domain.Tenant.Tenant>(tenant);

            await _testing.AddAsync<ApplicationDbContext, Domain.User.User>(new Domain.User.User
            {
                FirstName = "Test",
                LastName = "Tester",
                Username = "test@test.com",
                Email = "test@test.com",
                Password = "Abcdefgh1!",
                TenantId = tenant.Id
            });
        }

        internal async Task Cleanup()
        {
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
            var result = await _testing.SendAsync(new Login { Username = "test@test.com", Password = "Abcdefgh1!" });

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
            var act = async () => await _testing.SendAsync(new Login { Username = "abc@def.xyz", Password = "IncorrectPassword1!" });

            var result = await act.Should().ThrowAsync<ValidationException>();

            result.Which.Errors.Should().BeEquivalentTo(new Dictionary<string, string[]>
            {
                { "Username", new[] { "User does not exist." } }
            });
        }

        [Theory]
        [InlineData(null, "'Username' must not be empty.")]
        [InlineData("abc", "'Username' is not a valid email address.")]
        [InlineData("testtesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttest@test.com", "The length of 'Username' must be no more than 320 characters.")]
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
        [InlineData("a", "The length of 'Password' must be at least 8 characters.")]
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
    }
}
