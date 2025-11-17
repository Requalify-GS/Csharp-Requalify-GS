using Requalify.Hateoas;

namespace Requalify.DTOs.Responses
{
    public class PagedResponse<T> : ResourceResponse
    {
        public IEnumerable<T> Items { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
