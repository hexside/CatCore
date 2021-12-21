using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Interactions;
using CatCore.ClientAutocomplete;
using CatCore.Data;

namespace CatCore.ClientCommands;
public partial class PollCommands
{
	[SlashCommand("send", "sends a poll")]
	public async Task SendPoll(
	[Autocomplete(typeof(PollAutocompleteProvider))]
	[Summary("poll", "the poll to send")]
	string pollIdString)
	{
		Poll poll = await DBHelper.GetPollAsync(Convert.ToUInt64(pollIdString));
		await RespondAsync(embed:poll.GetEmbed().Build(), component: new ComponentBuilder()
			.WithButton("launch poll", $"poll.{poll.Id}.launch", ButtonStyle.Primary).Build());
	}
}
