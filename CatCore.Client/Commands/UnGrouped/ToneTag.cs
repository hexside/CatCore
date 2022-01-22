using Discord.Interactions;
using System.Threading.Tasks;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using CatCore.Data;
using Discord;
using System.Text.RegularExpressions;

namespace CatCore.Client.Commands;

public partial class UnGrouped
{
	public const string ToneTagRegex = @"[\\\/]([^\\\/ ]+)";
	public List<ToneTag> KnownTags { get; set; }
	[MessageCommand("Tone Tag")]
	public async Task ToneTag(SocketMessage message)
	{
		string content = message.Content ?? "empty-message";

		List<string> tags = Regex.Matches(content, ToneTagRegex)
			.SelectMany(x => x.Captures)
			.Select(x => x.Value[1..])
			.ToList();

		if (tags.Count < 1)
		{
			await RespondAsync("No tone tags were found in that message.", ephemeral: true);
			return;
		}

		Dictionary<string, ToneTag?> resolved = new();
		
		// null values are unresolved tags.
		tags.OnEach(x => resolved.Add(x, KnownTags.FirstOrDefault(y => (y.Matches(x).Item2 ?? "") == x)));
		resolved.Where(x => x.Value is null).OnEach(x => resolved.Remove(x.Key));
		List<ToneTagSource> credits = new();
		
		resolved.Values.Select(x => x.Source).OnEach(x => credits.Add(x));
		credits = credits.RemoveDuplicates(x => x.Title).ToList();

		if (resolved.Count >= 23)
		{
			await RespondAsync("The message has too many tags.", ephemeral: true);
			return;
		}

		string missingTags = "";
		string sources = "";
		
		tags.Where(x => !resolved.ContainsKey(x)).OnEach(x => missingTags += $" `{x}`, ");
			
		credits.Where(x => x is not null).OnEach(x => sources += x.Url is null
			? $"{x.Title}, "
			: $"[{x.Title}]({x.Url}), ");

		EmbedBuilder eb = new EmbedBuilder()
			.WithTitle("Tone Tags")
			.WithDescription(missingTags == ""
				? "All tags in the message are known."
				: $"unknown tags: {missingTags[..^2]}.")
			.AddField("Tag Sources", sources[..^2]);

		resolved.OnEach(x => eb.AddField(x.Key, x.Value.Description));
		await RespondAsync(embed: eb.Build(), ephemeral: true);
	}
}