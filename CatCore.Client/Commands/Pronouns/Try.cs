using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using CatCore.Data;
using CatCore.Client.Autocomplete;
using Discord.WebSocket;
using Discord;

namespace CatCore.Client.Commands;

public partial class PronounCommands
{ 
	[SlashCommand("try", "try out a pronoun on yourself")]
	public async Task TryPronoun(
		[Summary("pronoun", "the pronoun to try out")]
		[Autocomplete(typeof(PronounAutocompleteProvider))]
		string pronounId)
	{
		Pronoun pronoun = await DBHelper.GetPronounAsync(Convert.ToUInt64(pronounId));
		await RespondAsync($"Have you seen {Context.User.Mention}'s latest project? " +
			$"**{pronoun.Subject}** made a really pretty necklace that looks like **{pronoun.Possessive}** cats hugging." +
			$" I wish I spent more time on projects like **{pronoun.Object}**.", ephemeral: true, 
			allowedMentions:AllowedMentions.None);
	}
}
