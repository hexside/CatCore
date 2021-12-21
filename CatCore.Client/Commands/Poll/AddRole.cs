using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using CatCore.Client.Autocomplete;
using Discord;
using Discord.WebSocket;
using CatCore.Data;

namespace CatCore.Client.Commands
{
	public partial class PollCommands
	{
		[SlashCommand("add-role", "adds a role to a poll")]
		public async Task Add(
		[Summary("poll", "the poll to add the role to")]
		[Autocomplete(typeof(PollAutocompleteProvider))]
		Poll poll,
		[Summary("role", "the role to add")]
		IRole role,
		[Summary("description", "the text shown under the role name")]
		string description = null)
		{
			if (role.Position <= ((SocketGuildUser)Context.User).Roles.Select(x => x.Position).OrderByDescending(x => x).First() || 
				!((SocketGuildUser)Context.User).GuildPermissions.ManageRoles)
			{
				await RespondAsync($"You don't have permission to manage this role. Make sure it is below your highest role, and you have the `Manage Roles` permission");
				return;
			}
			
			if (role.Position <= ((SocketGuildChannel)Context.Channel).Guild.CurrentUser.Roles.Select(x => x.Position).OrderByDescending(x => x).First() ||
				!((SocketGuildChannel)Context.Channel).Guild.CurrentUser.GuildPermissions.ManageRoles)
			{
				await RespondAsync($"I don't have permission to manage this role. Make sure it is below my highest role, and I have the `Manage Roles` permission");
				return;
			}

			List<PollRole> roles = await DBHelper.GetPollRolesAsync(poll.Id);

			if (roles.Any(x => x.Id == role.Id))
			{
				await RespondAsync($"This poll already has {role.Mention}.", ephemeral:true, allowedMentions:AllowedMentions.None);
				return;
			}
			
			description ??= role.Name;

			PollRole newRole = new()
			{
				Description = description,
				PollId = poll.Id,
				RoleId = role.Id
			};

			await DBHelper.AddPollRoleAsync(newRole);

			await RespondAsync($"Added {role.Mention} to the poll.", ephemeral: true, allowedMentions: AllowedMentions.None);
		}
	}
}
