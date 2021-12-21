using System;
using System.Collections.Generic;
using System.Linq;
using Discord.WebSocket;
using System.Threading.Tasks;
using Discord.Interactions;
using CatCore.Data;
using CatCore.Client.Autocomplete;

namespace CatCore.Client.Commands
{
	public partial class PollCommands
	{
		[SlashCommand("remove-role", "deletes a role from a poll")]
		public async Task DeleteRole(
			[Summary(null, "The poll to delete the roll from.")]
			[Autocomplete(typeof(PollAutocompleteProvider))]
			Poll poll,
			[Summary(null, "The role to remove.")]
			SocketRole discordRole
			)
		{
			List<PollRole> roles = await DBHelper.GetPollRolesAsync(poll.Id);
			PollRole role = roles.Where(x => x.RoleId == discordRole.Id).FirstOrDefault();
			if (role is null)
			{
				await RespondAsync("That role is not in the poll.", ephemeral: true);
				return;
			}
			await DBHelper.RemovePollRoleAsync(role);

			await RespondAsync("Removed the role!", ephemeral: true);
		}
	}
}
