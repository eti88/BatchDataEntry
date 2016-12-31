using System.ComponentModel;

namespace BatchDataEntry.Models
{
    public enum TipoFileProcessato
    {
        [Description("Tiff File")]
        Tiff = 0,
        [Description("Pdf File")]
        Pdf = 1
    }
}
