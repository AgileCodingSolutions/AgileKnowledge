using System.Linq.Expressions;

namespace AgileKnowledge.Service.Helper
{
	public static class IQueryableExtensions
	{
		public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
		{
			return !condition ? query : query.Where<T>(predicate);
		}

		public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, int, bool>> predicate)
		{
			return condition
				? query.Where(predicate)
				: query;
		}
		public static IQueryable<T> PageBy<T>(this IQueryable<T> query, int pageNumber, int pageSize)
		{

			return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
		}

	}
}
