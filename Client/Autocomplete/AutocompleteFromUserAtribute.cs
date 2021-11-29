using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Autocomplete
{
	/// <summary>
	/// This attribute makes supported autocomplete providers
	/// autocomplete from the command's user's values instead of
	/// the entire db.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
	public class AutocompleteFromUserAtribute : Attribute
	{
		public bool Set = false;
		public AutocompleteFromUserAtribute() => Set = true;
	}
}
