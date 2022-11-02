namespace BlazorCleanArchitecture.Application.Common.Extensions
{
    public static class LinqExtensions
    {
        public static IQueryable<T> IfThen<T>(this IQueryable<T> query, bool condition, Func<IQueryable<T>, IQueryable<T>> thenDo)
        {
            if (condition) return thenDo.Invoke(query);
            return query;
        }

        public static IQueryable IfThen<T, R>(this T query, bool condition, Func<T, R> thenDo, Func<T, R> elseDo) where T : IQueryable where R : IQueryable
        {
            return (condition ? thenDo : elseDo).Invoke(query);
        }
    }
}
