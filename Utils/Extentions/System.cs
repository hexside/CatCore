using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
	public static partial class SystemExtentions
	{
		public static string ReplaceLast(this string original, string remove, string value)
		{
			int place = original.LastIndexOf(remove);

			if (place == -1) return original;

			original = original.Remove(place, remove.Length);
			original = original.Insert(place, value);
			return original;
		}
	}
}
