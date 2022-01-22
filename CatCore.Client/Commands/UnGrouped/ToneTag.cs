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

		tags.OnEach(x => System.Console.WriteLine(x));

		if (tags.Count < 1)
		{
			_ = await FollowupAsync("No tone tags were found in that message", ephemeral: true);
			return;
		}

		Dictionary<string, ToneTag> resolved = new();
		tags.OnEach(x => resolved.Add(x, KnownTags.First(y => (y.Matches(x).Item2 ?? "") == x)));
		List<ToneTagSource> credits = resolved.Values.Select(x => x.Source).RemoveDuplicates().ToList();

		if (resolved.Count >= 23)
		{
			await RespondAsync("too many tags", ephemeral: true);
			return;
		}

		string missingTags = "";
		string sources = "";
		tags.OnEach(x => missingTags += $" `{x}`, ");
		credits.OnEach(x => sources += x.Url is null
			? $"{x.Title}, "
			: $"[{x.Title}]({x.Url}), ");

		EmbedBuilder eb = new EmbedBuilder()
			.WithTitle("Tone Tags")
			.WithDescription(tags.Count < 1
				? "All tags in the message are known"
				: $"unknown tags: {missingTags[..^2]}")
			.AddField("Tag Sources", sources[..^2]);

		resolved.OnEach(x => eb.AddField(x.Key, x.Value.Description));
		await RespondAsync(embed: eb.Build(), ephemeral: true);
	}
}