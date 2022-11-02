using BlazorCleanArchitecture.Shared.Common.Models;
using System.Runtime.Serialization;

namespace BlazorCleanArchitecture.Shared.User.User
{
    [DataContract]
    public sealed class UserDto : AuditableEntityDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string PhoneNumber { get; set; }
        public bool Enabled { get; set; }
        public int LoginAttempts { get; set; }
        public bool Locked { get; set; }
    }
}
