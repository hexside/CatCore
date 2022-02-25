using CatCore.Client.Autocomplete;
using CatCore.Client.Modals;

namespace CatCore.Client.Commands;

[Group("poll", "commands for creating and managing polls.")]
[RequireUserPermission(GuildPermission.ManageRoles)]
[RequireBotPermission(GuildPermission.ManageRoles)]
public class PollCommands : InteractionModuleBase<CatCoreInteractionContext>
{
	[Group("role", "commands for creating and managing poll roles.")]
	[RequireUserPermission(GuildPermission.ManageRoles)]
	[RequireBotPermission(GuildPermission.ManageRoles)]
	public class PollRoleCommands : InteractionModuleBase<CatCoreInteractionContext>
	{
		[SlashCommand("add", "Adds a role to a poll.")]
		public async Task Add
		(
			[Summary("poll", "The poll to add the role to.")]
			[Autocomplete(typeof(PollAutocompleteProvider))] Poll poll,
			[Summary("role", "The role to add.")] IRole role,
			[Summary("description", "The text shown under the role name.")] string description = null,
			[Summary("emote", "The roles emoji.")] string emote = "<:amity:46264551874257006>"
		)
		{
			var guildUser = (SocketGuildUser)Context.User;
			int userPosition = guildUser.Roles
				.Select(x => x.Position)
				.OrderByDescending(x => x)
				.First();

			int botPosition = Context.Guild.CurrentUser.Roles
				.Select(x => x.Position)
				.OrderByDescending(x => x)
				.First(); ;

			bool isOwner = Context.Guild.OwnerId == Context.User.Id;

			if (!isOwner && role.Position >= userPosition)
			{
				await RespondAsync($"You don't have permission to manage this role. Make sure it is below your highest" +
					"role, and you have the **`Manage Roles`** permission", ephemeral: true);
				return;
			}

			if (role.Position >= botPosition)
			{
				await RespondAsync($"I don't have permission to manage this role. Make sure it is below my highest" +
					"role, and I have the **`Manage Roles`** permission", ephemeral: true);
				return;
			}

			if (!Emote.TryParse(emote, out _))
			{
				await RespondAsync($"That emoji is invalid, make sure it is a non-animated emoji, " +
					"contains no extra text, and matches the formatting data provided by " +
					"[discord](https://discord.com/developers/docs/reference#message-formatting-formats]", ephemeral: true);
			}

			if (description?.Length >= 100)
			{
				await RespondAsync("The roles description must be less than 100 characters.");
				return;
			}

			var roles = Context.Db.PollRoles.Where(x => x.PollId == poll.PollId);

			if (roles.Any(x => x.RoleId == role.Id))
			{
				await RespondAsync("That role is already in the poll, run `/poll role update` to change it.", ephemeral: true);
			}

			description ??= role.Name;

			PollRole newRole = new()
			{
				Description = description.Trim(),
				RoleId = role.Id,
				Poll = poll,
				Emote = emote
			};

			poll.Roles.Add(newRole);
			Context.Db.Polls.Update(poll);
			await Context.Db.SaveChangesAsync();

			await RespondAsync($"Added {role.Mention} to the poll.", ephemeral: true, allowedMentions: AllowedMentions.None);
		}

		[SlashCommand("remove", "Removes a role from a poll.")]
		public async Task Remove
		(
			[Summary(null, "The poll to delete the roll from.")]
			[Autocomplete(typeof(PollAutocompleteProvider))] Poll poll,
			[Summary(null, "The role to remove.")] SocketRole discordRole
		)
		{
			PollRole role = poll.Roles.FirstOrDefault(x => x.RoleId == discordRole.Id);
			if (role is null)
			{
				await RespondAsync("That role is not in the poll.", ephemeral: true);
				return;
			}
			poll.Roles.Remove(role);
			Context.Db.Guilds.Update(Context.DbGuild);
			await Context.Db.SaveChangesAsync();

			await RespondAsync("Removed the role!", ephemeral: true);
		}

		[SlashCommand("update", "Modifies a role in a poll")]
		public async Task Update
		(
			[Autocomplete(typeof(PollAutocompleteProvider))]
			[Summary(null, "The poll to update the role in.")] Poll poll,
			[Summary(null, "The role to update")] SocketRole discordRole,
			[Summary("emote", "The roles emoji.")] string? emote = null,
			[Summary("description", "The text shown under the role name.")] string? description = null
		)
		{
			PollRole role = poll.Roles.FirstOrDefault(x => x.RoleId == discordRole.Id);
			if (role is null)
			{
				await RespondAsync("That role is not in the poll.", ephemeral: true);
				return;
			}

			if (description.Length >= 100)
			{
				await RespondAsync("The roles description must be less than 100 characters.");
				return;
			}

			role.Description = description ?? role.Description;
			role.Emote = emote ?? role.Emote;

			Context.Db.PollRoles.Update(role);
			await Context.Db.SaveChangesAsync();

			await RespondAsync("Updated the role!", ephemeral: true);
		}
	}

	[SlashCommand("new", "Create a new poll.")]
	public async Task New
	(
		[Summary(null, "The smallest number of options a user can choose (defaults to 1 if too small).")] int? min = 0,
		[Summary(null, "The largest number of options a user can choose (defaults to total options if too large.)")]
		int? max = 0
	)
		=> await RespondWithModalAsync<PollModal>($"poll.create:{min},{max};");

	[ModalInteraction("poll.create:*,*;", true)]
	public async Task NewModal(string minStr, string maxStr, PollModal modal)
	{
		var min = int.Parse(minStr);
		var max = int.Parse(maxStr);

		Poll poll = new()
		{
			Title = modal.Name.Trim(),
			Description = modal.Description.Trim(),
			Footer = modal.Footer.Trim(),
			ImageUrl = modal.ImageUrl?.Trim(),
			Min = min,
			Max = max,
		};

		Context.DbGuild.Polls.Add(poll);
		Context.Db.Guilds.Update(Context.DbGuild);
		await Context.Db.SaveChangesAsync();

		await RespondAsync("Added the poll!", embed: poll.GetEmbed().Build(), ephemeral: true);
	}

	[SlashCommand("delete", "Deletes a poll.")]
	public async Task Delete
	(
		[Summary("poll", "The poll to delete.")]
		[Autocomplete(typeof(PollAutocompleteProvider))] Poll poll
	)
	{
		Context.DbGuild.Polls.Remove(poll);
		Context.Db.Guilds.Update(Context.DbGuild);
		await Context.Db.SaveChangesAsync();
		await RespondAsync("Deleted the poll!", embed: poll.GetEmbed().Build(), ephemeral: true);
	}

	[SlashCommand("send", "Sends a message users can use to launch a poll.")]
	public async Task SendPoll
	(
		[Autocomplete(typeof(PollAutocompleteProvider))]
		[Summary("poll", "The poll to send.")] Poll poll
	)
		=> await RespondAsync(embed: poll.GetEmbed().Build(), components: new ComponentBuilder()
			.WithButton("Launch poll!", $"poll.{poll.PollId}.launch", ButtonStyle.Primary).Build());

	[SlashCommand("update", "Modify an already created poll.")]
	public async Task UpdatePoll
	(
		[Autocomplete(typeof(PollAutocompleteProvider))]
		[Summary("poll", "The poll to update.")] Poll poll,
		[Summary(null, "The smallest number of options a user can choose (defaults to 1).")] int? min = null,
		[Summary(null, "The largest number of options a user can choose (defaults to total options if too large.)")]
		int? max = null
	)
	{
		poll.Min = min ?? poll.Min;
		poll.Max = max ?? poll.Max;

		var mb = new ModalBuilder()
			.WithTitle("Update Poll")
			.WithCustomId($"poll.{poll.PollId}.update:{poll.Min},{poll.Max};")
			.AddTextInput("name", "name", TextInputStyle.Short, "Enter your poll's name", 1, 256, true, poll.Title)
			.AddTextInput("description", "description", TextInputStyle.Paragraph, "Enter your poll's description",
				1, 256, true, poll.Description)
			.AddTextInput("footer", "footer", TextInputStyle.Paragraph, "Enter your poll's footer", 1, 1024, true,
				poll.Footer)
			.AddTextInput("image url", "image_url", TextInputStyle.Short, "Enter your poll's image url", 1, 1024,
				false, poll.ImageUrl.Length > 1 ? poll.ImageUrl : null);

		await RespondWithModalAsync(mb.Build());
	}

	[SlashCommand("color", "Sets the color of a polls embed")]
	public async Task UpdateColor
	(
		[Autocomplete(typeof(PollAutocompleteProvider))]
		[Summary("poll", "The poll to update")] Poll poll,
		[MaxValue(255)]
		[Summary("red", "The color's red value.")] int r,
		[MaxValue(255)]
		[Summary("green", "The color's green value.")] int g,
		[MaxValue(255)]
		[Summary("blue", "The color's blue value.")] int b
	)
	{
		poll.Color = new Color(r, g, b).RawValue;
		Context.Db.Polls.Update(poll);
		await Context.Db.SaveChangesAsync();
		await RespondAsync("Updated the polls color", embed: poll.GetEmbed().Build(), ephemeral: true);
	}

	[ModalInteraction("poll.*.update:*,*;", true)]
	public async Task UpdatePollModal(string pollIdStr, string minStr, string maxStr, PollModal modal)
	{
		int pollId = int.Parse(pollIdStr);
		Poll poll = Context.DbGuild.Polls.First(x => x.PollId == pollId);
		poll.Min = int.Parse(minStr);
		poll.Max = int.Parse(maxStr);
		poll.Title = modal.Name.Trim();
		poll.Description = modal.Description.Trim();
		poll.Footer = modal.Footer.Trim();
		poll.ImageUrl = modal.ImageUrl.Trim();
		Context.Db.Guilds.Update(Context.DbGuild);
		await Context.Db.SaveChangesAsync();

		await RespondAsync("Updated the poll", embed: poll.GetEmbed().Build(), ephemeral: true);
	}

	[ComponentInteraction("poll.*.result", true)]
	public async Task AddPollRole(string id, string[] values)
	{
		await (Context.Interaction as SocketMessageComponent).DeferLoadingAsync(ephemeral: true);

		Poll poll = await Context.Db.Polls
			.Include(x => x.Roles)
			.FirstAsync(x => x.PollId == int.Parse(id));

		SocketGuildUser user = Context.User as SocketGuildUser;
		var selectedRoles = values.Select(x => Convert.ToUInt64(x));

		List<ulong> FinalRoles = user.Roles.Select(x => x.Id).ToList();
		FinalRoles.RemoveAll(x => poll.Roles.Select(x => x.RoleId).Contains(x) && !selectedRoles.Contains(x));
		FinalRoles.AddRange(selectedRoles.Where(x => !FinalRoles.Contains(x)));
		FinalRoles.Remove(Context.Guild.EveryoneRole.Id);

		await user.ModifyAsync(x => x.RoleIds = FinalRoles);

		await FollowupAsync("Updated your roles.", allowedMentions: AllowedMentions.None);
	}

	[ComponentInteraction("poll.*.launch", true)]
	public async Task RespondWithPoll(string idStr)
	{
		int id = int.Parse(idStr);

		Poll poll = await Context.Db.Polls
			.Include(x => x.Roles)
			.FirstAsync(x => x.PollId == id);

		var roles = poll.Roles;

		roles = roles.Count > 20
			? roles.GetRange(0, 20)
			: roles;

		int min = poll.Min > roles.Count || poll.Min < 0
			? 1
			: poll.Min;

		int max = poll.Max > roles.Count || poll.Max < 1
			? roles.Count
			: poll.Max;

		ComponentBuilder cb = new();
		SelectMenuBuilder sb = new SelectMenuBuilder()
			.WithCustomId($"poll.{id}.result")
			.WithPlaceholder("Select your roles!")
			.WithMinValues(min)
			.WithMaxValues(max);

		var guildRoles = Context.Guild.Roles;
		var userRoles = (Context.User as IGuildUser).RoleIds;

		roles.OnEach(x => sb.AddOption(guildRoles
			.First(y => y.Id == x.RoleId).Name, x.RoleId.ToString(), x.Description, Emote.Parse(x.Emote),
				isDefault: userRoles.Contains(x.RoleId)));


		roles = roles.Count > 20
			? roles.GetRange(0, 20)
			: roles;
		cb.WithSelectMenu(sb);

		await RespondAsync(embed: poll.GetEmbed().Build(), components: cb.Build(), ephemeral: true);
	}
}
