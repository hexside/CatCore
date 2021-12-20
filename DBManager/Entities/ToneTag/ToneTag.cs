using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBManager
{

	public class ToneTag
	{
		public ToneTagSource? Source { get; set; }
		public string DefaultName { get; set; }
		public string DefaultShortName { get; set; }
		public string Description { get; set; }
		public List<string> Aliases { get; set; }
		public List<string> ShortAliases { get; set; }

		private List<string> _allValues
		{
			get
			{
				List<string> value = Aliases;
				ShortAliases.ForEach(x => value.Add(x));
				return value;

			}
		}

		public ListMatchesResult Matches(string value)
		{
			bool matches = false;
			string matchesValue = null;

			_allValues.OnEach(x =>
			{
				if (!matches && string.Equals(x, value, StringComparison.InvariantCultureIgnoreCase))
				{
					matches = true;
					matchesValue = x;
				}
			});

			return new (matches, matchesValue);
		}
	}
}
