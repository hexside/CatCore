using System;
using System.Collections.Generic;
using System.Linq;
using Discord.WebSocket;
using System.Threading.Tasks;
using Discord.Interactions;
using DBManager;
using Client.Autocomplete;

namespace Client.Commands
{
	public partial class PollCommands
	{
		[SlashCommand("remove-role", "deletes a role from a poll")]
		public async Task DeleteRole(
			[Summary("poll", "The poll to delete the roll from.")]
			[Autocomplete(typeof(PollAutocompleteProvider))]
			string pollId,
			[Summary(null, "The role to remove.")]
			SocketRole discordRole
			)
		{
			Poll poll = await DBHelper.GetPollAsync(Convert.ToUInt64(pollId));
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
