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
		ulong? internalId = (await DBHelper.GetUserAsync(user.Id))?.InternalId;

		if (internalId is null)
		{
			await DBHelper.AddUserAsync(user.Id, false);
			internalId = (await DBHelper.GetUserAsync(user.Id)).InternalId;
		}

		List<Pronoun> pronouns = await DBHelper.GetPronounsAsync(internalId.Value);

		string responce = pronouns.Count switch
		{
			0 => $"{user.Mention} did not specify their pronouns, have them run `/pronoun add` to set them",
			_ => $@"{user.Mention}'s pronouns are {string.Concat(pronouns.Select(x => $"**{x.ToString(0x11)}**, "))
					[..^2].ReplaceLast(",", " and")}"
		};

		await Context.Interaction.RespondAsync(responce, ephemeral:true, allowedMentions:AllowedMentions.None);
	}
}