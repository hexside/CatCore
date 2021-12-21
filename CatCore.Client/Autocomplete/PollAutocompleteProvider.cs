using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using DBManager;

namespace Client.Autocomplete;
internal class PollAutocompleteProvider : AutocompleteHandler
{
	public DBHelper DBHelper { get; set; }
	public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context,
		IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
	{
		List<Poll> polls = await DBHelper.GetGuildPollsAsync(context.Guild.Id);
		string currentValue = autocompleteInteraction.Data.Current.Value.ToString();

		polls = polls
			.Where(x => x.ToString(true)
			.Contains(currentValue, StringComparison.InvariantCultureIgnoreCase))
			.OrderBy(x => x.Id)
			.ToList();

		polls = polls.Count > 20
			? polls.GetRange(0, 20)
			: polls;

		return AutocompletionResult.FromSuccess(polls.Select(x => new AutocompleteResult(x.ToString(false),
			x.Id.ToString())));
	}
}
