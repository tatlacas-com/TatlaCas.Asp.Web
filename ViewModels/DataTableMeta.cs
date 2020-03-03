
namespace TatlaCas.Asp.Web.ViewModels
{
    /// <summary>
    /// Meta object should contain the metadata that required for the datatable pagination to work.
    /// </summary>
    public class DataTableMeta
    {
        /// <summary>
        /// The current page number.
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// Total number of pages available in the server.
        /// </summary>
        public int Pages { get; set; }
        /// <summary>
        /// Total records per page.
        /// </summary>
        public int Perpage { get; set; }
        /// <summary>
        /// Total all records number available in the server
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// Sort type asc for ascending and desc for descending.
        /// </summary>
        public string Sort { get; set; }
        /// <summary>
        /// Field name which the sort should be applied to.
        /// </summary>
        public string Field { get; set; }
    }
}