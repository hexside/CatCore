using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using CatCore.Data;
using CatCore.Client.Autocomplete;

namespace CatCore.Client.Commands
{
	public partial class PollCommands
	{
		[SlashCommand("delete", "deletes a new poll")]
		public async Task Delete(
			[Summary("poll", "The poll to delete.")]
			[Autocomplete(typeof(PollAutocompleteProvider))]
			Poll poll)
		{	
			await DBHelper.RemovePollAsync(poll);

			await RespondAsync("Removed the poll!", embed: poll.GetEmbed().Build(), ephemeral: true);
		}
	}
}
