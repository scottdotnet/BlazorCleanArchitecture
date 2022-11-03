using BlazorCleanArchitecture.Domain.User;
using BlazorCleanArchitecture.Infrastructure.Data;
using BlazorCleanArchitecture.Shared.Authentication.Commands;
using BlazorCleanArchitecture.Shared.Common.Exceptions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BlazorCleanArchitecture.Application.IntegrationTests.Authentication.Commands
{
    [Collection("BlazorCleanArchitecture.Application.IntegrationTests")]
    public sealed class ResetPasswordTests : IAsyncLifetime
    {
        private readonly TestFixture _testing;

        private readonly PasswordReset Reset = new PasswordReset { };
        private readonly Domain.User.User User = new Domain.User.User
        {
            FirstName = "Test",
            LastName = "Tester",
            Username = "test@test.com",
            Email = "test@test.com",
            Password = "Abcdefgh1!"
        };

        public ResetPasswordTests(TestFixture testing)
            => _testing = testing;

        internal async Task Setup()
        {
            var tenant = new Domain.Tenant.Tenant
            {
                Name = "Test",
                Domain = _testing.CurrentUser.Username.Split("@")[1]
            };

            await _testing.AddAsync<ApplicationDbContext, Domain.Tenant.Tenant>(tenant);

            User.TenantId = tenant.Id;

            await _testing.AddAsync<ApplicationDbContext, Domain.User.User>(User);

            Reset.UserId = User.Id;

            await _testing.AddAsync<ApplicationDbContext, PasswordReset>(Reset);
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
        public async Task ResetPasswordSuccessfully()
        {
            var result = await _testing.SendAsync(new ResetPassword
            {
                ResetId = Reset.Id,
                Username = "test@test.com",
                Password = "Abcdefgh2!",
                ConfirmPassword = "Abcdefgh2!"
            });

            result.Should().BeTrue();

            var resetCount = await _testing.CountAsync<ApplicationDbContext, PasswordReset>();

            resetCount.Should().Be(0);

            var user = await _testing.FindAsync<ApplicationDbContext, Domain.User.User>(User.Id);

            user!.Password.Should().Be("Abcdefgh2!");
        }

        [Fact]
        public async Task ResetIdNull()
        {
            var act = async () => await _testing.SendAsync(new ResetPassword
            {
                Username = "test@test.com",
                Password = "Abcdefgh1!",
                ConfirmPassword = "Abcdefgh1!!"
            });

            var result = await act.Should().ThrowAsync<ValidationException>();

            result.Which.Errors.Should().BeEquivalentTo(new Dictionary<string, string[]>
            {
                { "ResetId", new[] { "'Reset Id' must not be empty." } }
            });
        }

        [Fact]
        public async Task UsernameNull()
        {
            var act = async () => await _testing.SendAsync(new ResetPassword
            {
                ResetId = Reset.Id,
                Password = "Abcdefgh1!",
                ConfirmPassword = "Abcdefgh1!"
            });

            var result = await act.Should().ThrowAsync<ValidationException>();

            result.Which.Errors.Should().BeEquivalentTo(new Dictionary<string, string[]>
            {
                { "Username", new[] { "'Username' must not be empty." } }
            });
        }

        [Fact]
        public async Task PasswordNull()
        {
            var act = async () => await _testing.SendAsync(new ResetPassword
            {
                ResetId = Reset.Id,
                Username = "test@test.com",
                ConfirmPassword = "Abcdefgh1!!"
            });

            var result = await act.Should().ThrowAsync<ValidationException>();

            result.Which.Errors.Should().BeEquivalentTo(new Dictionary<string, string[]>
            {
                { "Password", new[] { "'Password' must not be empty." } }
            });
        }

        [Fact]
        public async Task ConfirmPasswordNull()
        {
            var act = async () => await _testing.SendAsync(new ResetPassword
            {
                ResetId = Reset.Id,
                Username = "test@test.com",
                Password = "Abcdefgh1!",
            });

            var result = await act.Should().ThrowAsync<ValidationException>();

            result.Which.Errors.Should().BeEquivalentTo(new Dictionary<string, string[]>
            {
                { "ConfirmPassword", new[] { "Passwords do not match." } }
            });
        }

        [Fact]
        public async Task PasswordsDontMatch()
        {
            var act = async () => await _testing.SendAsync(new ResetPassword
            {
                ResetId = Reset.Id,
                Username = "test@test.com",
                Password = "Abcdefgh1!",
                ConfirmPassword = "Abcdefgh1!!"
            });

            var result = await act.Should().ThrowAsync<ValidationException>();

            result.Which.Errors.Should().BeEquivalentTo(new Dictionary<string, string[]>
            {
                { "ConfirmPassword", new[] { "Passwords do not match." } }
            });
        }

        [Fact]
        public async Task PasswordIncorrect()
        {
            var act = async () => await _testing.SendAsync(new ResetPassword
            {
                ResetId = Reset.Id,
                Username = "test@test.com",
                Password = "abcdefgh1!",
                ConfirmPassword = "Abcdefgh1!"
            });

            var result = await act.Should().ThrowAsync<ValidationException>();

            result.Which.Errors.Should().BeEquivalentTo(new Dictionary<string, string[]>
            {
                { "Password", new[] { "Password must contain atleast 1 upper-case character." } }
            });
        }

        [Fact]
        public async Task CorrectResetIdIncorrectUsernaame()
        {
            var act = async () => await _testing.SendAsync(new ResetPassword
            {
                ResetId = Reset.Id,
                Username = "abc@def.xyz",
                Password = "Abcdefgh1!",
                ConfirmPassword = "Abcdefgh1!"
            });

            var result = await act.Should().ThrowAsync<ValidationException>();

            result.Which.Errors.Should().BeEquivalentTo(new Dictionary<string, string[]>
            {
                { "Username", new[] { "User doesn't match reset ID." } }
            });
        }

        [Fact]
        public async Task IncorrectResetIdCorrectUsername()
        {
            var act = async () => await _testing.SendAsync(new ResetPassword
            {
                ResetId = Guid.NewGuid(),
                Username = "test@test.com",
                Password = "Abcdefgh1!",
                ConfirmPassword = "Abcdefgh1!"
            });

            var result = await act.Should().ThrowAsync<ValidationException>();

            result.Which.Errors.Should().BeEquivalentTo(new Dictionary<string, string[]>
            {
                { "ResetId", new[] { "Reset ID does not exist." } }
            });
        }
    }
}
