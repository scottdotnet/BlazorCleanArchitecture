namespace BlazorCleanArchitecture.Shared.Common.Models
{
    public abstract class AuditableEntityDto
    {
        public DateTime Created { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? Modified { get; set; }
        public int? ModifiedBy { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
