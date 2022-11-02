namespace BlazorCleanArchitecture.Domain.Common
{
    public abstract class AuditableEntity
    {
        public DateTime Created { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? Modified { get; set; }
        public int? ModifiedBy { get; set; }
        public byte[] RowVersion { get; set; }
        public int TenantId { get; set; }
    }
}
