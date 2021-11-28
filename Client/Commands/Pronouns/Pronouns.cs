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
	[UserCommand("Pronouns")]
	public async Task Pronouns(SocketUser user)
	{
		List<Pronoun> pronouns = await DBHelper.GetPronounsAsync((await DBHelper.GetUserAsync(user.Id)).InternalId);

		string responce = pronouns.Count switch
		{
			0 => $"{user.Username} did not specify their pronouns, have them run `/pronoun add` to set them",
			_ => $"{ user.Username }'s pronouns are {string.Concat(pronouns.Select(x => x.ToString() + ", "))}"[..^2]
		};

		await Context.Interaction.RespondAsync(responce);
	}
}