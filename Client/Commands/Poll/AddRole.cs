using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using Client.Autocomplete;
using Discord;
using DBManager;

namespace Client.Commands
{
	public partial class PollCommands
	{
		[SlashCommand("add-role", "adds a role to a poll")]
		public async Task Add(
		[Summary("poll", "the poll to add the role to")]
		[Autocomplete(typeof(PollAutocompleteProvider))]
		string pollName,
		[Summary("role", "the role to add")]
		IRole role,
		[Summary("description", "the text shown under the role name")]
		string description = "")
		{
			ulong pollId = Convert.ToUInt64(pollName);

			Poll poll = await DBHelper.GetPollAsync(pollId);

			List<PollRole> roles = await DBHelper.GetPollRolesAsync(pollId);

			if (roles.Any(x => x.Id == role.Id))
			{
				await RespondAsync($"this poll already has {role.Mention}.", ephemeral:true);
				return;
			}

			PollRole newRole = new()
			{
				Description = description,
				PollId = pollId,
				RoleId = role.Id
			};

			await DBHelper.AddPollRoleAsync(newRole);

			await RespondAsync($"Added {role.Mention} to the poll.", ephemeral: true);
		}
	}
}
