using System;
using System.Collections.Generic;

namespace System.Linq
{
	public static partial class CottageDwellingAdditions
	{
		public static void OnEach<t>(this IEnumerable<t> e, Action<t> a)
		{
			foreach (t i in e)
				a.Invoke(i);
		}

		public static bool Multiple<t>(this IEnumerable<t> e, Func<t, bool> a)
		{
			int c = 0;
			e.OnEach(x => c += a.Invoke(x) ? 1 : 0);
			return c >= 2;
		}

		public static bool AtLeast<t> (this IEnumerable<t> e, int m, Func<t, bool> a)
		{
			int c = 0;
			e.OnEach(x => c += a.Invoke(x) ? 1 : 0);
			return c >= m;
		}

		public static IEnumerable<T> RemoveDuplicates<T>(this IEnumerable<T> l)
		{
			List<T> r = new();
			l.OnEach(x => { if (!r.Contains(x)) r.Add(x); });
			return r;
		}
		
		public static IEnumerable<Tin> RemoveDuplicates<Tin, Tret>(this IEnumerable<Tin> e, Func<Tin, Tret> c)
		{
			List<Tin> r = new();
			List<Tret> v = new();
			e.OnEach(x =>
			{
				Tret a = c(x);
				if (!v.Contains(a))
				{
					v.Add(a);
					r.Add(x);
				}
			});

			return r;
		}
	}
}
