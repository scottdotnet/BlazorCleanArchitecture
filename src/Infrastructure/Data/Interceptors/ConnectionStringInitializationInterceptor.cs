using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCleanArchitecture.Infrastructure.Data.Interceptors
{
    public sealed class ConnectionStringInitializationInterceptor : DbConnectionInterceptor
    {
        private readonly IConfiguration _configuration;

        public ConnectionStringInitializationInterceptor(IConfiguration configuration)
            => _configuration = configuration;

        public override async ValueTask<InterceptionResult> ConnectionOpeningAsync(DbConnection connection, ConnectionEventData eventData, InterceptionResult result, CancellationToken cancellationToken = default)
        {
            connection.ConnectionString = _configuration.GetConnectionString("Default");

            return result;
        }
    }
}
