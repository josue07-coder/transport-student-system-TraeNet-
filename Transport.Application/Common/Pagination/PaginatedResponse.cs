namespace Transport.Application.Common.Pagination
{
    public class PaginatedResponse<T>
    {
        public PaginatedResponse(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
        {
            Items = items.ToList();
            TotalCount = totalCount;
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = pageSize < 1 ? 10 : pageSize;
            TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
        }

        public int PageNumber { get; }
        public int PageSize { get; }
        public int TotalCount { get; }
        public int TotalPages { get; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        public IReadOnlyCollection<T> Items { get; }
    }
}
