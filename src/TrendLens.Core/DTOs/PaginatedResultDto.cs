using System.ComponentModel.DataAnnotations;

namespace TrendLens.Core.DTOs;

/// <summary>
/// Generic paginated result container
/// </summary>
/// <typeparam name="T">Type of items in the result</typeparam>
public sealed class PaginatedResultDto<T>
{
    /// <summary>
    /// Items in the current page
    /// </summary>
    public IEnumerable<T> Items { get; init; } = Enumerable.Empty<T>();

    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    [Range(1, int.MaxValue)]
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Number of items per page
    /// </summary>
    [Range(1, 100)]
    public int PageSize { get; init; } = 10;

    /// <summary>
    /// Total number of items across all pages
    /// </summary>
    [Range(0, int.MaxValue)]
    public int TotalCount { get; init; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    /// Number of the first item on the current page
    /// </summary>
    public int FirstItemOnPage => (PageNumber - 1) * PageSize + 1;

    /// <summary>
    /// Number of the last item on the current page
    /// </summary>
    public int LastItemOnPage => Math.Min(PageNumber * PageSize, TotalCount);
}
