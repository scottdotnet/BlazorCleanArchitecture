using System.Runtime.CompilerServices;

namespace BlazorCleanArchitecture.Shared.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class PermissionAttribute : Attribute
    {
        public string Permssion { get; init; }
        public string Role { get; init; } = string.Empty;

        public PermissionAttribute([CallerMemberName] string permission = "")
            => Permssion = permission;
    }
}
