using IbnelveApp.Domain.Enums;

namespace IbnelveApp.Domain.Entities
{
    public abstract class EntidadeBase
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public DateTime? DataAlteracao { get; set; }
        public Status Status { get; set; } = Status.Ativo;
    }

}
