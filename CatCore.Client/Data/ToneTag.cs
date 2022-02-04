namespace CatCore.Data;

public class ToneTag
{
	public ToneTagSource? Source { get; set; }
	public string DefaultName { get; set; }
	public string DefaultShortName { get; set; }
	public string Description { get; set; }
	public List<string> Aliases { get; set; }
	public List<string> ShortAliases { get; set; }

	public List<string> AllValues
	{
		get
		{
			List<string> value = Aliases;
			value.AddRange(ShortAliases);
			value.Add(DefaultName);
			value.Add(DefaultShortName);
			return value;
		}
	}

	public bool Matches(string value)
	{
		bool matches = false;

		AllValues.OnEach(x =>
		{
			if (!matches && string.Equals(x, value, StringComparison.InvariantCultureIgnoreCase))
			{
				matches = true;
			}
		});

		return matches;
	}
}
