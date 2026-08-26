namespace ProductManagementAPI.Common
{
    // T represents the type of data, such as ProductResponseDTO.
    public class PagedResult<T>
    {


        // Contains the records returned for the current page.
        // IReadOnlyCollection prevents callers from modifying the collection.
        public IReadOnlyCollection<T> Items { get; }

        // Gets the current page number.
        public int PageNumber { get; }

        // Gets the number of records requested per page.
        public int PageSize { get; }

        // Gets the total number of records available.
        public int TotalRecords { get; }

        // Gets the total number of pages calculated from
        // the total records and page size.
        public int TotalPages { get; }

        // Returns true when a page exists before the current page.
        public bool HasPreviousPage => PageNumber > 1;

        // Returns true when another page exists after the current page.
        public bool HasNextPage => PageNumber < TotalPages;
        public PagedResult(
            IReadOnlyCollection<T> items,
            int pageNumber,
            int pageSize,
            int totalRecords)
        {
            Items= items;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalRecords = totalRecords;

            // Calculates the total number of pages.
            // Math.Ceiling ensures that a partially filled page is counted.
            // For example, 21 records with a page size of 10 require 3 pages.
            // If no records are available, the total number of pages is zero.
            TotalPages = totalRecords == 0
                ? 0
                : (int)Math.Ceiling(totalRecords / (double)PageSize);


        }
    }
}
