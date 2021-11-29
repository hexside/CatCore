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
	[SlashCommand("new", "creates a new pronoun")]
	public async Task New(
	[Summary("subject", "They in \"They are my best friend.\"")]
	string subject,
	[Summary("object", "Them in \"I love going to the park with them.\"")]
	string @object,
	[Summary("possesive", "Theirs in \"That sandwich is theirs.\"")]
	string possesive,
	[Summary("reflexive", "Themself in \"They used to go on walks by themself; but now we mostly go together.\"")]
	string reflexive)
	{
		Pronoun pronoun = new()
		{
			Subject = subject,
			Object = @object,
			Possesive = possesive,
			Reflexive = reflexive
		};

		if (await DBHelper.DuplicatePronoun(pronoun))
		{
			await RespondAsync("that pronoun already exists", ephemeral: true);
			return;
		}

		await DBHelper.AddPronounAsync(pronoun);
		await RespondAsync($"created the pronoun **{pronoun.ToString(0x13)}**", ephemeral:true);
	}
}