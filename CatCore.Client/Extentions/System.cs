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

		public static int GetInt(this bool inpt) => inpt ? 1 : 0;

		public static bool GetBool(this int inpt) => inpt switch
		{
			1 => true,
			0 => false,
			_ => throw new ArgumentOutOfRangeException(nameof(inpt), inpt, "Value most be 0 or 1")
		};
	}
}
