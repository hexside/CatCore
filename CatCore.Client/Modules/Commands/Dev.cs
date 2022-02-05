namespace CatCore.Client.Commands;

[Group("dev", "dev commands.")]
public partial class DevCommands : InteractionModuleBase<CatCoreInteractionContext>
{
	public CatCoreContext db { get; set; }
	[SlashCommand("say", "Echo a message.")]
	public async Task Say
	(
		[Summary("message", "The message to send.")] string message,
		[Summary("as-file", "Send the message as a file instead of text.")] bool messageAsFile = false,
		[Summary("ephemeral", "Should the message be ephemeral?")] bool ephemeral = false
	)
	{
		await DeferAsync(ephemeral);
		if (messageAsFile)
		{
			using MemoryStream ms = new();
			using StreamWriter sw = new(ms);
			sw.Write(message);
			sw.Flush();
			ms.Position = 0;

			await Context.Interaction.FollowupWithFileAsync(ms, "file.txt");
		}
		else
		{
			await Context.Interaction.FollowupAsync(message);
		}
	}
	[SlashCommand("ping", "Check the current client latency.")]
	public async Task Ping()
		=> await Context.Interaction.RespondAsync($"the current ping is {Context.Client.Latency}", ephemeral: true);

	[SlashCommand("throw", "Throw an unhandled exception.")]
	public async Task Throw
	(
		[Summary("defer", "How should the interaction be deferred.")] DeferType defer,
		[Summary("reason", "What should the exception message be.")] string reason = "idk, you tell me"
	)
	{
		if (defer == DeferType.DeferEphemerally) await Context.Interaction.DeferAsync(true);
		if (defer == DeferType.Defer) await Context.Interaction.DeferAsync(false);
		throw new Exception(reason);
	}

	public enum DeferType
	{
		DontDefer,
		Defer,
		DeferEphemerally
	}

	[SlashCommand("message", "Message a User")]
	public async Task MessageUserAsync
	(
		SocketUser discordUser,
		string title,
		string description,
		string footer = "This an official message from the CatCore developers.",
		string imageUrl = "https://cdn.discordapp.com/attachments/839193628525330482/939395506709348413/PXL_20211217_014154431.jpg?passthrew=true",
		string adminMessage = "testing."
	)
	{
		Message message = new()
		{
			Title = title,
			Description = description,
			ImageUrl = imageUrl,
			Footer = footer,
			AdminMessage = adminMessage
		};

		var user = await db.Users.FirstAsync(x => x.DiscordID == discordUser.Id);
		user.Messages.Add(new(message, user));
		db.Users.Update(user);
		await db.SaveChangesAsync();

		await RespondAsync($"Messaged {discordUser.Mention}.");
	}
	
	[SlashCommand("patchnotes", "Message all users")]
	public async Task SendPatchnotesAsync
	(
		string title,
		string description,
		string footer = "This an official message from the CatCore developers.",
		string imageUrl = "https://cdn.discordapp.com/attachments/839193628525330482/939395506709348413/PXL_20211217_014154431.jpg?passthrew=true",
		string adminMessage = "testing."
	)
	{
		Message message = new()
		{
			Title = title,
			Description = description,
			ImageUrl = imageUrl,
			Footer = footer,
			AdminMessage = adminMessage
		};

		await message.AddToAsync(await db.Users.ToListAsync(), db);
		await db.SaveChangesAsync();

		await RespondAsync("Messaged **all users** (yikes..).");
	}
}
