using System.Text.RegularExpressions;

namespace CatCore.Client;

public class Utils
{
	public static Dictionary<string, ToneTag> ResolveToneTags(List<ToneTag> known, string message)
	{
		var tags = Regex.Matches(message, @"[\\\/](?<tag>[^\\\/\s]+)")
			.SelectMany(x => x.Groups["tag"].Captures)
			.Select(x => x.Value);

		//FIXME: double ToDictionary is inefficient, probably a cleaner way to do this.
		Dictionary<string, ToneTag> resolved = tags
			.RemoveDuplicates()
			.ToDictionary(x => x, x => known.FirstOrDefault(y => y.Matches(x)))
			.Where(x => x.Value is not null)
			.ToDictionary(x => x.Key, x => x.Value);

		return resolved;
	}
}
