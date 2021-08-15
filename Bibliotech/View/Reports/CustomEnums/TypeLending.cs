using System.ComponentModel;

namespace Bibliotech.View.Reports.CustomEnums
{
    public enum TypeLending
    {
        [Description("AMBOS")]
        Both = 0,
        [Description("PEGOU")]
        PickUp = 1,
        [Description("DEVOLVEU")]
        Returned = 2,
    }
}
