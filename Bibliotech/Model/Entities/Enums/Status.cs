using System.ComponentModel;

namespace Bibliotech.Model.Entities.Enums
{
    public enum Status
    {
        [Description("Inativo")]
        Inactive = 0,
        [Description("Ativo")]
        Active = 1,
        [Description("Emprestado")]
        Loaned = 2,
        [Description("Estoque")]
        Stock = 3,
        [Description("Extraviado")]
        Lost = 4,
        [Description("Atrasado")]
        Late = 5,
    }
}
