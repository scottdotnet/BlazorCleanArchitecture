using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BlazorCleanArchitecture.Application.IntegrationTests
{
    [CollectionDefinition("BlazorCleanArchitecture.Application.IntegrationTests")]
    public sealed class TestFixtureCollection : ICollectionFixture<TestFixture>
    {
    }
}
