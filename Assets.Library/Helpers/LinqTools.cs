using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assets.Library.Helpers
	{
	public static class LinqTools
		{
		//https://extensionmethod.net/csharp/iqueryable-t/tolistasync
		/// <summary>
		/// Async create of a System.Collections.Generic.List<T> from an 
		/// System.Collections.Generic.IQueryable<T>.
		/// </summary>
		/// <typeparam name="T">The type of the elements of source.</typeparam>
		/// <param name="list">The System.Collections.Generic.IEnumerable<T> 
		/// to create a System.Collections.Generic.List<T> from.</param>
		/// <returns> A System.Collections.Generic.List<T> that contains elements 
		/// from the input sequence.</returns>
		public static Task<List<T>> ToListAsync<T>(this IQueryable<T> list) {
			return Task.Run(() => list.ToList());
			}

// Not sure if this will work!
		public static Task<List<T>> ToListAsync<T>(this IEnumerable<T> list) {
			return Task.Run(() => list.ToList());
			}



		// https://stackoverflow.com/questions/10632776/fastest-way-to-remove-duplicate-value-from-a-list-by-lambda
		/// IEnumerable<Foo> distinctList = sourceList.DistinctBy(x => x.FooName);
		/// 
			public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
				this IEnumerable<TSource> source,
				Func<TSource, TKey> keySelector)
				{
				var knownKeys = new HashSet<TKey>();
				return source.Where(element => knownKeys.Add(keySelector(element)));
				}
			}
   	}
	
