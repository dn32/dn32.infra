using System.ComponentModel;


namespace dn32.infra
{
    public class FiltersAndTerm
    {
        [Description("Query Filters")]
        public DnFiltro[] Filters { get; set; }

        [Description("The properties whose valor will be compared")]
        public string[] Properties { get; set; }

        [Description("The term to compare with the property valor")]
        public string Term { get; set; }

        [Description("The tolerance of comparing term and property valor")]
        public int Tolerance { get; set; }
    }
}