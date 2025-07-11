namespace IbnelveApp.Domain.Interfaces
{
    public interface IMultiTenant { 
        public Guid TenantId { get; set; } 
    }
}
