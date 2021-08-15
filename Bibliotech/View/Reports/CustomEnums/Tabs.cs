using System.ComponentModel;

namespace Bibliotech.View.Reports.CustomEnums
{
    public enum Tabs
    {
        [Description("EMPRÉSTIMOS")]
        Lendings = 0,
        [Description("LEITORES")]
        Lectors = 1,
        [Description("LIVROS")]
        Books = 2,
    }
}
