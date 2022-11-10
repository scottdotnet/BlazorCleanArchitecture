using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCleanArchitecture.Infrastructure.Data.Interceptors
{
    public sealed class CacheDataInterceptor : DbCommandInterceptor
    {
        private readonly IMemoryCache _cache;

        public CacheDataInterceptor(IMemoryCache cache)
            => _cache = cache;

        public override async ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
            DbCommand command,
            CommandEventData eventData,
            InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = default)
        {
            if (_cache.TryGetValue(command.CommandText, out DataTable dataTable))
            {
                command.CommandText = "-- Cache";
                result = InterceptionResult<DbDataReader>.SuppressWithResult(dataTable.CreateDataReader());
            }

            return await new ValueTask<InterceptionResult<DbDataReader>>(result);
        }

        public override async ValueTask<DbDataReader> ReaderExecutedAsync(
            DbCommand command,
            CommandExecutedEventData eventData,
            DbDataReader result,
            CancellationToken cancellationToken = default)
        {
            var cache = await _cache.GetOrCreateAsync(command.CommandText, async e =>
            {
                var dt = new DataTable();
                dt.Load(result);

                return dt;
            });

            return cache.CreateDataReader();
        }
    }
}
