using Discord.Interactions;
using System.Threading.Tasks;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using DBManager;
using Discord;

namespace Client.Commands;
public partial class UnGrouped
{
	public List<ToneTag> Tags { get; set; }
	[MessageCommand("Tone Tag")]
	public async Task ToneTag(SocketMessage message)
	{
		string content = message.Content ?? "empty-message";
		await DeferAsync(true);

		// tokenization tools
		List<string> unfilteredOutput = new();
		string currentToken = "";
		bool inToken = false;
		content.ToCharArray().OnEach(x =>
		{
			// if we are starting a tag
			if (x == '/')
			{
				// mark that we are in a token
				inToken = true;
				// add the current tag to unfilteredOutput
				if (currentToken != "") unfilteredOutput.Add(currentToken);
				currentToken = "";
			}
			// if we are not starting a tag
			else
			{
				// if we are in a tag
				if (inToken)
				{
					// if the tag is ending
					if (x is ' ' or '/' or '\n')
					{
						// mark that we aren't in a tag
						inToken = false;
						unfilteredOutput.Add(currentToken);
						currentToken = "";
					}
					// if the tag is not ending
					else currentToken += x.ToString().ToLower();
				}
			}
		});
		if (currentToken != "") unfilteredOutput.Add(currentToken);

		// remove duplicate values
		List<string> output = new();
		unfilteredOutput.OnEach(x => { if (!output.Contains(x)) output.Add(x); });

		if (output.Count < 1)
		{
			_ = await FollowupAsync("No tone tags were found in that message", ephemeral: true);
			return;
		}

		List<(ToneTag Tag, string Name)> resolvedTags = new();
		List<string> removeResolved = new();

		List<ToneTagSource> tagCredits = new();

		// resolve tags
		// on each tag in the message loop threw every tag we know
		output.OnEach(x => Tags.OnEach(y =>
			{
				// if the tags match mark the tag from the message as known and  
				// resolve the tag it matches
				(_, string? tag) = y.Matches(x);
				if (tag is string nnTag)
				{
					resolvedTags.Add((y, nnTag));
					removeResolved.Add(x);

					// resolve tag credits 
					if (y.Source != null &&
					!tagCredits.Any(x => x.Title == y.Source.Title))
						tagCredits.Add(y.Source);
				}
			}
		));

		// remove tags we don't need
		removeResolved.OnEach(x => output.Remove(x));

		if (resolvedTags.Count >= 23)
		{
			await FollowupAsync("too many tags", ephemeral: true);
			return;
		}

		string missingTags = "";
		string sources = "";
		output.OnEach(x => missingTags += $" `{x}`, ");
		tagCredits.OnEach(x => sources += x.Url is null
			? $"{x.Title}, "
			: $"[{x.Title}]({x.Url ?? "invalid url"}), ");

		EmbedBuilder eb = new EmbedBuilder()
			.WithTitle("Tone Tags")
			.WithDescription(output.Count < 1
				? "All tags in the message are known"
				: $"unknown tags: {missingTags[..^2]}")
			.AddField("Tag Sources", sources[..^2]);

		resolvedTags.OnEach(x => eb.AddField(x.Name, x.Tag.Description));
		await FollowupAsync(embed: eb.Build(), ephemeral: true);

	}
}