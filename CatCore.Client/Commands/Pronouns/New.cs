using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using CatCore.Data;

namespace CatCore.Client.Commands;

public partial class PronounCommands
{
	[SlashCommand("new", "creates a new pronoun")]
	public async Task New(
	[Summary("subject", "They in \"They are my best friend.\"")]
	string subject,
	[Summary("object", "Them in \"I love going to the park with them.\"")]
	string @object,
	[Summary("possessive", "Theirs in \"That sandwich is theirs.\"")]
	string possessive,
	[Summary("reflexive", "Themself in \"They used to go on walks by themself; but now we mostly go together.\"")]
	string reflexive)
	{
		Pronoun pronoun = new()
		{
			Subject = subject,
			Object = @object,
			Possessive = possessive,
			Reflexive = reflexive
		};

		if (await DBHelper.DuplicatePronoun(pronoun))
		{
			await RespondAsync("that pronoun already exists", ephemeral: true);
			return;
		}

		await DBHelper.AddPronounAsync(pronoun);
		await RespondAsync($"created the pronoun **{pronoun:s/o/p/r}**", ephemeral:true);
	}
}