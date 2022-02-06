using CatCore.Client.Autocomplete;

namespace CatCore.Client.Commands;

public class Groupless : InteractionModuleBase<CatCoreInteractionContext>
{	
	public List<ToneTag> KnownTags { get; set; }

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
