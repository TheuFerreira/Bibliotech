using System.ComponentModel;

namespace Bibliotech.View.Reports.CustomEnums
{
    public enum Period
    {
        [Description("DIA")]
        Day = 0,
        [Description("MÊS")]
        Mount = 1,
        [Description("ANO")]
        Year = 2,
        [Description("CUSTOMIZADO")]
        Custom = 3
    }
}
