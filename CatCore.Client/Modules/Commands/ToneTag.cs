using System.Text.RegularExpressions;
using CatCore.Client.Autocomplete;

namespace CatCore.Client.Commands;

[Group("tone-tag", "tone-tag")]
public class ToneTagCommands : InteractionModuleBase<CatCoreInteractionContext>
{
	public List<ToneTag> KnownTags { get; set; }

	[SlashCommand("resolve", "Resolve the tone tags in a message.")]
	public async Task Resolve
	(
		[Summary("message-id", "The id of a message in this channel (see https://dis.gd/findmyid for info).")]
string messageId
	)
	{
		if (!Regex.IsMatch(messageId, @"^\s*\d+\s*$"))
		{
			await RespondAsync("Invalid message id, make sure it is a number.", ephemeral: true);
			return;
		}

		IMessage message = await Context.Channel.GetMessageAsync(Convert.ToUInt64(messageId),
			options: new() { RetryMode = RetryMode.Retry502 });

		if (message is null)
		{
			await RespondAsync("A message with that id is not in the current channel.", ephemeral: true);
			return;
		}

		string content = message.Content ?? "empty-message";

		var tags = Utils.ResolveToneTags(KnownTags, content);

		if (tags.Count < 1)
		{
			await RespondAsync("No tone tags were found in that message.", ephemeral: true);
			return;
		}

		if (tags.Count >= 23)
		{
			await RespondAsync("The message has too many tags.", ephemeral: true);
			return;
		}

		EmbedBuilder eb = new EmbedBuilder()
			.WithTitle("Tone Tags");

		tags.OnEach(x => eb.AddField(x.Key, x.Value.Description));
		await RespondAsync(embed: eb.Build(), ephemeral: true);
	}

	[SlashCommand("search", "Find a specified tone tag by it's name or alias.")]
	public async Task Search
	(
		[Autocomplete(typeof(ToneTagAutocompleteProvider))]
		[Summary("tag", "The tone tag to search for.")] string tagInput
	)
	{
		ToneTag? tag = KnownTags.FirstOrDefault(x => x.Matches(tagInput));
		if (tag is null)
		{
			await RespondAsync("Unknown tone tag, try again or use the autocomplete suggestions.", ephemeral: true);
			return;
		}

		string footer = (tag.Source.Url ?? "") == ""
			? tag.Source.Title
			: $"[{tag.Source.Title}]({tag.Source.Url})";

		var eb = new EmbedBuilder()
			.WithTitle(tagInput)
			.WithDescription(tag.Description)
			.WithFooter(footer);

		await RespondAsync(embed: eb.Build(), ephemeral: true);
	}

	[MessageCommand("Tone Tag")]
	public async Task ToneTag(SocketMessage message)
	{
		string content = message.Content ?? "empty-message";

		var tags = Utils.ResolveToneTags(KnownTags, content);

		if (tags.Count < 1)
		{
			await RespondAsync("No tone tags were found in that message.", ephemeral: true);
			return;
		}

		if (tags.Count >= 23)
		{
			await RespondAsync("The message has too many tags.", ephemeral: true);
			return;
		}

		EmbedBuilder eb = new EmbedBuilder()
			.WithTitle("Tone Tags");

		tags.OnEach(x => eb.AddField(x.Key, x.Value.Description));
		await RespondAsync(embed: eb.Build(), ephemeral: true);
	}
}
