using BlazorCleanArchitecture.Shared.Common.Interfaces;

namespace BlazorCleanArchitecture.Shared.Services
{
    public sealed class DateTimeService : IDateTimeService
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
