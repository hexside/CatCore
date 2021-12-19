using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;

namespace Client.Commands;

public partial class PollCommands
{
	[ComponentInteraction("poll.*.result", true)]
	public async Task AddPollRole(string id)
	{
		await RespondAsync(id);
	}
}
