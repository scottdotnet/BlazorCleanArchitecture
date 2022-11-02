namespace BlazorCleanArchitecture.Domain.Tenant
{
    public sealed class Tenant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Domain { get; set; }
    }
}
