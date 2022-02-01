using System;
using Discord.Interactions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using CatCore.Data;
using Discord;
using System.Text.RegularExpressions;
using CatCore.Client.Autocomplete;

namespace CatCore.Client.Commands;

public partial class ToneTagCommands
{
	[SlashCommand("search", "Find a specified tone tag by it's name or alias.")]
	public async Task Search
	(
		[Summary("tag", "The tone tag to search for.")]
		[Autocomplete(typeof(ToneTagAutocompleteProvider))]
		string tagInput
	)
	{
		ToneTag? tag = KnownTags.FirstOrDefault(x => x.Matches(tagInput));
		if (tag is null)
		{
			await RespondAsync("Unknown tone tag, try again or use the autocomplete suggestions.", ephemeral: true);
			return;
		}

		string footer = (tag.Source.Url ?? "") == "" 
			? tag.Source.Title 
			: $"[{tag.Source.Title}]({tag.Source.Url})";

		var eb = new EmbedBuilder()
			.WithTitle(tagInput)
			.WithDescription(tag.Description)
			.WithFooter(footer);

		await RespondAsync(embed: eb.Build(), ephemeral: true);
	}
}