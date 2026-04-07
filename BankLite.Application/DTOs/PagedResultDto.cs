using System.Collections.Generic;

namespace BankLite.Application.DTOs
{
    public class PagedResultDto<T>
    {
        public IEnumerable<T> Items { get; set; } = null!;
        public int TotalCount { get; set; }
        public int Page {  get; set; }
        public int PageSize { get; set; }
    }
}
