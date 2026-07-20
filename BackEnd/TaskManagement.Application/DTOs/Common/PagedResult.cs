namespace TaskManagement.Application.DTOs.Common;

/// <summary>One page of results plus the totals a pager needs to render.</summary>
public class PagedResult<T>
{
    // The rows for the requested page.
    public IReadOnlyList<T> Items { get; set; } = new List<T>();
    // Total rows matching the filters, across all pages.
    public int TotalCount { get; set; }
    // 1-based page number that was returned.
    public int Page { get; set; }
    // Rows per page.
    public int PageSize { get; set; }
    // Total number of pages available for the current filters.
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}
