using System.Linq.Expressions;

namespace SpeedElems.Library;

/// <summary>
/// Linq extensions
/// </summary>
public static class LinqExtensions
{
    /// <summary>
    /// Between expression
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="source"></param>
    /// <param name="keySelector"></param>
    /// <param name="low"></param>
    /// <param name="high"></param>
    /// <returns></returns>
    public static IEnumerable<TSource> Between<TSource, TKey>(this IEnumerable<TSource> source, Expression<Func<TSource, TKey>> keySelector, TKey low, TKey high) where TKey : IComparable<TKey>
    {
        Expression key = Expression.Invoke(keySelector, keySelector.Parameters.ToArray());
        Expression lowerBound = Expression.GreaterThanOrEqual(key, Expression.Constant(low));
        Expression upperBound = Expression.LessThanOrEqual(key, Expression.Constant(high));
        Expression and = Expression.AndAlso(lowerBound, upperBound);
        Expression<Func<TSource, bool>> lambda = Expression.Lambda<Func<TSource, bool>>(and, keySelector.Parameters);
        return source.Where(lambda.Compile());
    }
}