using CatCore.Client.Autocomplete;

namespace CatCore.Client.Commands;

public class Groupless : InteractionModuleBase<CatCoreInteractionContext>
{	
	public List<ToneTag> KnownTags { get; set; }
	public CatCoreContext Db { get; set; }

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

	[SlashCommand("mail", "Search your mail for a message from the devs.")]
	public async Task Mail
	(
		[Summary("filter", "What mail do you want to see?")] MailSearchType searchType,
		[Autocomplete(typeof(UserMessageAutocompleteProvider))]
		[Summary(null, "Search for a message.")] UserMessage message
	)
	{
		message.IsRead = true;
		Db.UserMessages.Update(message);
		await Db.SaveChangesAsync();
		
		await RespondAsync(embed: message.Message.GetEmbed(), ephemeral: true);
	}
	public enum MailSearchType
	{
		Unread,
		Read,
		All
	}
}
