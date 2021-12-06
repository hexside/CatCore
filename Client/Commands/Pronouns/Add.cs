using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using DBManager;
using Client.Autocomplete;
using Discord.WebSocket;
using Discord;

namespace Client.Commands;

public partial class PronounCommands
{
	[SlashCommand("add", "add a pronoun to your profile")]
	public async Task Add(
		[Summary("pronoun", "the pronoun to add")]
		[Autocomplete(typeof(PronounAutocompleteProvider))]
		string pronounId)
	{
		Pronoun pronoun = await DBHelper.GetPronounAsync(Convert.ToUInt64(pronounId));
		User user = await DBHelper.GetUserAsync(Context.User.Id);
		await DBHelper.AddUserPronounAsync(pronoun.Id, user.InternalId);
		await RespondAsync($"added **{pronoun:s/o/p}** to your profile", ephemeral: true,
			allowedMentions: AllowedMentions.None);
	}
}
