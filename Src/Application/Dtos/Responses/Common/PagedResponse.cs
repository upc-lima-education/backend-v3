namespace Backend.Src.Application.Dtos.Responses.Common;
public record PagedResponse<T>
{
    public IReadOnlyList<T> Items { get; init; }
    public int TotalItems { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
    public PagedResponse(
        IReadOnlyList<T> items,
        int totalItems,
        int page,
        int pageSize
    )
    {
        Items = items;
        TotalItems = totalItems;
        Page = page;
        PageSize = pageSize;
    }
}