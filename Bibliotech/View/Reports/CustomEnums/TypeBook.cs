using System.ComponentModel;

namespace Bibliotech.View.Reports.CustomEnums
{
    public enum TypeBook
    {
        [Description("TÍTULO")]
        Title = 0,
        [Description("EDITORA")]
        PublishingCompany = 1,
        [Description("AUTOR(ES)")]
        Authors = 2,
    }
}
