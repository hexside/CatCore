using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using DBManager;
using Client.Autocomplete;

namespace Client.Commands
{
	public partial class PollCommands
	{
		[SlashCommand("delete", "deletes a new poll")]
		public async Task Delete(
			[Summary("poll", "The poll to delete.")]
			[Autocomplete(typeof(PollAutocompleteProvider))]
			string pollId)
		{
			Poll poll = await DBHelper.GetPollAsync(Convert.ToUInt64(pollId));
			
			await DBHelper.RemovePollAsync(poll);

			await RespondAsync("Removed the poll!", embed: poll.GetEmbed().Build(), ephemeral: true);
		}
	}
}
