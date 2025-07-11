using IbnelveApp.Domain.Enums;
using System.Text.Json.Serialization;

namespace IbnelveApp.Domain.Entities
{
    public abstract class EntidadeControladaBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public DateTime? DataAlteracao { get; set; }
        public Status Status { get; set; } = Status.Ativo;
        public bool IsDeleted { get; set; } = false;
        public Guid TenantId { get; set; }
        [JsonIgnore] // Opcional, mas bom para evitar redundância na serialização
        public Tenant Tenant { get; set; }
    }

}
