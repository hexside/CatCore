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
	[SlashCommand("remove", "remove a pronoun to your profile")]
	public async Task Remove(
		[Summary("remove", "the pronoun to remove")]
		[Autocomplete(typeof(PronounAutocompleteProvider))]
		[AutocompleteFromUserAtribute]
		string pronounId)
	{
		Pronoun pronoun = await DBHelper.GetPronounAsync(Convert.ToUInt64(pronounId));
		User user = await DBHelper.GetUserAsync(Context.User.Id);
		await DBHelper.RemoveUserPronounAsync(pronoun.Id, user.InternalId);
		await RespondAsync($"removed **{pronoun:s/o/p/r}** from your profile", ephemeral: true,
			allowedMentions: AllowedMentions.None);
	}
}
