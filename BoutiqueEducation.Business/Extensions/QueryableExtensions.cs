using BoutiqueEducation.Business.Models;
using Microsoft.EntityFrameworkCore;

namespace BoutiqueEducation.Business.Extensions;

public static class QueryableExtensions
{
    public static async Task<PagedResponse<T>> ToPagedResponseAsync<T>(this IQueryable<T> source, int pageNumber, int pageSize)
    {
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 10 : pageSize;

        var count = await source.CountAsync();
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResponse<T>(items, count, pageNumber, pageSize);
    }
}
