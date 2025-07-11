using IbnelveApp.Domain.Interfaces;
using System.Text.Json.Serialization;

namespace IbnelveApp.Domain.Entities
{

    public class Equipamento : EntidadeControladaBase, ISoftDelete, IMultiTenant
    {
        public string Nome { get; set; }
        public string Observacoes { get; set; }
        public string NumeroControle { get; set; }

    }

}
