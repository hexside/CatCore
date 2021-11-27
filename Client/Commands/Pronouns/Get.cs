using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DBManager;

namespace Client.Commands;

public partial class Pronouns
{
	[SlashCommand("get", "get another users pronouns")]
	public async Task Get(
	[Summary("user", "the user to get pronouns from")]
	SocketUser user)
	{
		string pronoun = 
			string.Concat((
			await DBHelper.GetPronounsAsync((await DBHelper.GetUserAsync(user.Id)).InternalId))
			.Select(x => x.ToString() + ", "));

		await Context.Interaction.RespondAsync($"{user.Username}'s pronouns are {pronoun[..^2]}");
	}
}