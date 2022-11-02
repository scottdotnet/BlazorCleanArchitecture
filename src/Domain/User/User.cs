using BlazorCleanArchitecture.Domain.Common;

namespace BlazorCleanArchitecture.Domain.User
{
    public sealed class User : AuditableEntity
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string MobileNumber { get; set; }
        public string PhoneNumber { get; set; }
        public bool Enabled { get; set; }
        public int LoginAttempts { get; set; }
        public bool Locked { get; set; }
    }
}
