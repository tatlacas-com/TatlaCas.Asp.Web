namespace TatlaCas.Asp.Web.ViewModels
{
    public class DataTableParams
    {
        public DataTableMeta Pagination { get; set; }
        public DataTableQuery Query { get; set; }
        public DataTableSort Sort { get; set; }
    }
}