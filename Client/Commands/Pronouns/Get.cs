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

public partial class PronounCommands
{
	[SlashCommand("get", "get another users pronouns")]
	public async Task Get(
	[Summary("user", "the user to get pronouns from")]
	SocketUser user)
		=> await Pronouns(user);
}