namespace BoutiqueEducation.Business.Models;

public sealed class PagedResponse<T>
{
    public IReadOnlyList<T> Data { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPageCount { get; }

    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPageCount;

    public PagedResponse(IReadOnlyList<T> items, int count, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber <= 0 ? 1 : pageNumber;
        PageSize = pageSize <= 0 ? 10 : pageSize;
        TotalCount = count;
        TotalPageCount = (int)Math.Ceiling(count / (double)PageSize);
        Data = items;
    }
}
