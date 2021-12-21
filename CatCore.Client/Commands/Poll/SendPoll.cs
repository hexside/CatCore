using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Interactions;
using CatCore.Client.Autocomplete;
using CatCore.Data;

namespace CatCore.Client.Commands;
public partial class PollCommands
{
	[SlashCommand("send", "sends a poll")]
	public async Task SendPoll(
	[Autocomplete(typeof(PollAutocompleteProvider))]
	[Summary("poll", "the poll to send")]
	Poll poll)
	{
		await RespondAsync(embed:poll.GetEmbed().Build(), component: new ComponentBuilder()
			.WithButton("launch poll", $"poll.{poll.Id}.launch", ButtonStyle.Primary).Build());
	}
}
