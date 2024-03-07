namespace mvcApp.Models
{
    public partial class SearchRequest
    {
        public string searchText { get; set; } = "";
        public int pageNumber { get; set; }
    }
}