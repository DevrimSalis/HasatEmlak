using HasatEmlak.Models.Entities;

namespace HasatEmlak.Models.ViewModels
{
    public class PropertyListViewModel
    {
        public IEnumerable<Property> Properties { get; set; }
        public SearchViewModel SearchModel { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
