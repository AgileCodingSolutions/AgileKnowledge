namespace AgileKnowledge.Service.Mappings
{
    [Serializable]
    public class PagedResultDto<T> : ListResultDto<T>
    {
        /// <inheritdoc />
        public long TotalCount { get; set; }

        public int PageSize { get; set; }

        public int PageNumber { get; set; }

        /// <summary>
        /// Creates a new <see cref="PagedResultDto{T}"/> object.
        /// </summary>
        public PagedResultDto()
        {

        }

        /// <summary>
        /// Creates a new <see cref="PagedResultDto{T}"/> object.
        /// </summary>
        /// <param name="totalCount">Total count of Items</param>
        /// <param name="items">List of items in current page</param>
        public PagedResultDto(long totalCount, IReadOnlyList<T> items)
            : base(items)
        {
            TotalCount = totalCount;
        }

        /// <summary>
        /// Creates a new <see cref="PagedResultDto{T}"/> object.
        /// </summary>
        /// <param name="totalCount">Total count of Items</param>
        /// <param name="items">List of items in current page</param>
        public PagedResultDto(long totalCount, int pageSize, int pageNumber, IReadOnlyList<T> items)
            : base(items)
        {
            TotalCount = totalCount;
            PageSize = pageSize;
            PageNumber = pageNumber;
        }
    }
}
