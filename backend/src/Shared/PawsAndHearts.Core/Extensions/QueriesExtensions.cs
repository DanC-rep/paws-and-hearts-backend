using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PawsAndHearts.Core.Models;

namespace PawsAndHearts.Core.Extensions;

public static class QueriesExtensions
{
    public static async Task<PagedList<T>> ToPagedList<T>(
        this IQueryable<T> source,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await source.CountAsync(cancellationToken);

        var items = await source
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        return new PagedList<T>
        {
            Items = items,
            PageSize = pageSize,
            Page = page,
            TotalCount = totalCount
        };
    }
    
    public static PagedList<T> ToPagedList<T>(
        this IEnumerable<T> source,
        int page,
        int pageSize)
    {
        var list = source.ToList();
        var totalCount = list.Count;

        var items = list
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        
        return new PagedList<T>
        {
            Items = items,
            PageSize = pageSize,
            Page = page,
            TotalCount = totalCount
        };
    }

    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> source,
        bool condition,
        Expression<Func<T, bool>> predicate)
    {
        return condition ? source.Where(predicate) : source;
    }
}