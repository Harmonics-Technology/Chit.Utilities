
using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace Chit.Utilities;
public static class IQueryableExtensions
{
    private static IMapper _mapper;
    private static IConfigurationProvider _configurationProvider;
    public static void Configure(IMapper mapper, IConfigurationProvider configurationProvider)
    {
        _mapper = mapper;
        _configurationProvider = configurationProvider;
    }

    public static void InitIQueryableExtensions(IMapper mapper, IConfigurationProvider configurationProvider)
    {
        _mapper = mapper;
        _configurationProvider = configurationProvider;
    }

    public static PagedCollection<T> ToPagedCollection<U, T>(this IQueryable<U> source, PagingOptions options, Link link)
    {

        var pagedQuery = source.Skip(options.Offset.Value).Take(options.Limit.Value);
        var mappedView = pagedQuery.ProjectTo<T>(_configurationProvider);
        var pagedCollection = PagedCollection<T>.Create(link, mappedView.ToArray(), source.Count(), options);
        return pagedCollection;
    }

    public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> source, PagingOptions options)
    {
        return source.Skip(options.Offset.Value).Take(options.Limit.Value);
    }

    public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string sortDirection, string propertyName) where T : class 
    {
        if (sortDirection == "asc")
        {
            return source.OrderBy(GetPropertyExpression<T>(propertyName));
        }
        else
        {
            return source.OrderByDescending(GetPropertyExpression<T>(propertyName));
        }
    }

    private static Expression<Func<T, object>> GetPropertyExpression<T>(string propertyName)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, propertyName);
        var propertyExpression = Expression.Convert(property, typeof(object));
        return Expression.Lambda<Func<T, object>>(propertyExpression, parameter);
    }

    // public static IQueryable<T> ApplySearch<T>(this IQueryable<T> source, string[] propertyNames, string searchQuery)
    // {
    //     if (string.IsNullOrEmpty(searchQuery))
    //     {
    //         return source;
    //     }

    //     var query = searchQuery.ToLower();
    //     var parameter = Expression.Parameter(typeof(T), "x");
    //     var predicate = Expression.MakeBinary(ExpressionType.OrElse, Expression.Constant(true), Expression.Constant(false));

    //     foreach (var propertyName in propertyNames)
    //     {
    //         var property = Expression.Property(parameter, propertyName);
    //         var propertyValue = Expression.Call(Expression.Call(property, "ToString", null), "ToLower", null);
    //         var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
    //         var containsExpression = Expression.Call(propertyValue, containsMethod, Expression.Constant(query));
    //         predicate = Expression.Or(predicate, containsExpression);
    //     }

    //     var lambda = Expression.Lambda<Func<T, bool>>(predicate, parameter);
    //     return source.Where(lambda);
    // }

    public static IQueryable<T> ApplySearch<T>(this IQueryable<T> source, string[] propertyNames, string searchQuery)
    {
        if (string.IsNullOrEmpty(searchQuery))
        {
            return source;
        }

        var query = searchQuery.ToLower();
        var parameter = Expression.Parameter(typeof(T), "x");
        var anotherPredicate = Expression.MakeBinary(ExpressionType.OrElse, Expression.Constant(true), Expression.Constant(false));

        // Initial predicate with the first property check (converted to bool)
        // var predicate = Expression.Call(
        //     Expression.Call(
        //         Expression.Property(parameter, propertyNames[0]),
        //         "ToString", null),
        //     "ToLower", null),
        // "Contains", new[] { typeof(string) }, Expression.Constant(query));

        var predicate = Expression.Call(
            Expression.Call(
                Expression.Property(parameter, propertyNames[0]),
                "ToString", null),
            "ToLower", null);

        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        predicate = Expression.Call(predicate, containsMethod, Expression.Constant(query));

        for (int i = 1; i < propertyNames.Length; i++)
        {
            var propertyName = propertyNames[i];
            var property = Expression.Property(parameter, propertyName);
            var propertyValue = Expression.Call(Expression.Call(property, "ToString", null), "ToLower", null);
            var containsExpression = Expression.Call(propertyValue, containsMethod, Expression.Constant(query));

            // Convert the containsExpression (string) to bool using ToLower().Contains("here") ? true : false
            var convertedContainsExpression = Expression.Convert(
                Expression.Condition(
                    containsExpression,
                    Expression.Constant(true),
                    Expression.Constant(false)
                ),
                typeof(bool));

            // Now you can use OR Else with the converted bool expression
            // predicate = Expression.OrElse(predicate, convertedContainsExpression);
            // predicate = Expression.Not(Expression.AndAlso(Expression.Not(predicate), Expression.Not(convertedContainsExpression)));
            anotherPredicate = Expression.OrElse(anotherPredicate, convertedContainsExpression);
            

        }

        var lambda = Expression.Lambda<Func<T, bool>>(anotherPredicate, parameter);
        return source.Where(lambda);
    }

    private static string GetPropertyValue<T>(T obj, string propertyName)
    {
        var propertyInfo = typeof(T).GetProperty(propertyName);
        var propertyValue = propertyInfo.GetValue(obj)?.ToString();
        return propertyValue ?? string.Empty;
    }
}
