using CatCore.Client.Autocomplete;

namespace CatCore.Client.Commands;

[Group("dev", "dev commands.")]
[DebugModeCommand]
public partial class DevCommands : InteractionModuleBase<CatCoreInteractionContext>
{
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

	[Group("message", "message users")]
	public class DevMailCommands : InteractionModuleBase<CatCoreInteractionContext>
	{
		[SlashCommand("to-user", "Message a User")]
		public async Task MessageUserAsync
		(
			SocketUser discordUser,
			string title,
			string description,
			string footer = "This an official message from the CatCore developers.",
			string imageUrl = "https://cdn.discordapp.com/attachments/839193628525330482/939395506709348413/PXL_20211217_014154431.jpg?passthrew=true",
			string adminMessage = "testing.",
			int color = 0x2E8B57
		)
		{
			Message message = new()
			{
				Title = title,
				Description = description,
				ImageUrl = imageUrl,
				Footer = footer,
				AdminMessage = adminMessage,
				Color = color
			};

			var user = await Context.Db.Users.FirstAsync(x => x.DiscordID == discordUser.Id);
			user.Messages.Add(new(message, user));
			Context.Db.Users.Update(user);
			await Context.Db.SaveChangesAsync();

			await RespondAsync($"Messaged {discordUser.Mention}.");
		}

		[SlashCommand("to-group", "Message all users")]
		public async Task SendPatchnotesAsync
		(
			[Autocomplete(typeof(MessageGroupAutocompleteProvider))] MessageGroup group,
			string title,
			string description,
			string footer = "This an official message from the CatCore developers.",
			string imageUrl = "https://cdn.discordapp.com/attachments/839193628525330482/939395506709348413/PXL_20211217_014154431.jpg?passthrew=true",
			string adminMessage = "testing.",
			int color = 0x2E8B57
		)
		{
			Message message = new()
			{
				Title = title,
				Description = description,
				ImageUrl = imageUrl,
				Footer = footer,
				AdminMessage = adminMessage,
				Color = color
			};

			await group.MessageAllUsersAsync(message, Context.Db);
			await Context.Db.SaveChangesAsync();

			await RespondAsync("Messaged **all users** (yikes..).");
		}

		[SlashCommand("create-group", "Creates a new group.")]
		public async Task CreateGroupAsync
		(
			string name,
			bool isPublic
		)
		{
			MessageGroup group = new()
			{
				Name = name,
				IsPublic = isPublic
			};

			await Context.Db.MessageGroups.AddAsync(group);
			await Context.Db.SaveChangesAsync();

			await RespondAsync("Created the group", ephemeral: true);
		}

		[SlashCommand("group-member", "Manages Group Membership")]
		public async Task GroupMember
		(
			[Autocomplete(typeof(MessageGroupAutocompleteProvider))] MessageGroup group,
			GroupMemberAction action,
			[Summary("user", null)] SocketUser discordUser
		)
		{
			var user = await Context.Db.Users
				.Include(x => x.MessageGroups)
					.ThenInclude(x => x.VisiableTo)
				.FirstAsync(x => x.DiscordID == discordUser.Id);
			switch (action)
			{
				case GroupMemberAction.Add:
					{
						if (!user.MessageGroups.Contains(group))
						{
							user.MessageGroups.Add(group);
							await Context.Db.SaveChangesAsync();
							await RespondAsync($"Added {discordUser.Mention} to **{group.Name}**.", ephemeral: true);
						}
						else
							await RespondAsync($"{discordUser.Mention} was already in **{group.Name}**.",
								ephemeral: true);
					}
					break;
				case GroupMemberAction.Remove:
					{
						if (user.MessageGroups.Contains(group))
						{
							user.MessageGroups.Remove(group);
							await Context.Db.SaveChangesAsync();
							await RespondAsync($"Removed {discordUser.Mention} from **{group.Name}**.",
								ephemeral: true);
						}
						else
							await RespondAsync($"{discordUser.Mention} was not in **{group.Name}**.", ephemeral: true);
					}
					break;
				case GroupMemberAction.MakeVisiable:
					{
						if (!group.VisiableTo.Contains(user))
						{
							group.VisiableTo.Add(user);
							await Context.Db.SaveChangesAsync();
							await RespondAsync($"{discordUser.Mention} can now see **{group.Name}**.",
								ephemeral: true);
						}
						else
							await RespondAsync($"{discordUser.Mention} can already see **{group.Name}**.",
								ephemeral: true);
					}
					break;
				case GroupMemberAction.MakeInvisible:
					{
						if (group.VisiableTo.Contains(user))
						{
							group.VisiableTo.Remove(user);
							await Context.Db.SaveChangesAsync();
							await RespondAsync($"{discordUser.Mention} can no longer see **{group.Name}**.",
								ephemeral: true);
						}
						else
							await RespondAsync($"{discordUser.Mention} could never see **{group.Name}**.",
								ephemeral: true);
					}
					break;
			}
		}

		public enum GroupMemberAction
		{
			Add,
			Remove,
			MakeVisiable,
			MakeInvisible
		}
	}
}
