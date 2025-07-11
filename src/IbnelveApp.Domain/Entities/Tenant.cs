namespace IbnelveApp.Domain.Entities
{
    public class Tenant
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public bool IsAtivo { get; set; } = true;
    }
}
