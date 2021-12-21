using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using CatCore.Client.Autocomplete;
using Discord;
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
			description ??= role.Name;

			List<PollRole> roles = await DBHelper.GetPollRolesAsync(poll.Id);

			if (roles.Any(x => x.Id == role.Id))
			{
				await RespondAsync($"this poll already has {role.Mention}.", ephemeral:true);
				return;
			}

			PollRole newRole = new()
			{
				Description = description,
				PollId = poll.Id,
				RoleId = role.Id
			};

			await DBHelper.AddPollRoleAsync(newRole);

			await RespondAsync($"Added {role.Mention} to the poll.", ephemeral: true);
		}
	}
}
