using System.ComponentModel.DataAnnotations;

namespace videochatspa.Server.Mappings.BaseDto
{
    [Serializable]
    public class PagedAndSortedResultRequestDto
    {
        public string? Filter { get; set; }
        public string? Sorting { get; set; }
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; }
        [Range(1, int.MaxValue)]
        public int PageSize { get; set; }
    }
}
