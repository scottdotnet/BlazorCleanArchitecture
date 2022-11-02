using BlazorCleanArchitecture.Domain.Common;

namespace BlazorCleanArchitecture.Domain.User
{
    public sealed class PasswordReset : AuditableEntity
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }

        public User User { get; set; }
    }
}
