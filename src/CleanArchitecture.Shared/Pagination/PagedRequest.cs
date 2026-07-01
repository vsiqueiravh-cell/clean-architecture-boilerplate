namespace CleanArchitecture.Shared.Pagination;

public sealed record PagedRequest(int Page = 1, int PageSize = 25)
{
    public int NormalizedPage => Math.Max(Page, 1);

    public int NormalizedPageSize => Math.Clamp(PageSize, 1, 100);

    public int Skip => (NormalizedPage - 1) * NormalizedPageSize;
}
