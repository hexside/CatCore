using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using DBManager;

namespace Client.Autocomplete;
internal class PronounAutocompleteProvider : AutocompleteHandler
{
	public DBHelper DBHelper { get; set; }
	public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, 
		IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
	{
		List<Pronoun> pronouns = new();
		string currentValue = autocompleteInteraction.Data.Current.Value.ToString();

		// gets and sorts the pronouns, didn't even know you could chain conditionals like this
		pronouns = parameter.Attributes.Any(x => x is AutocompleteFromUserAtribute)
			? currentValue == ""
                ? (await DBHelper.GetUsersPronounsAsync((await DBHelper.GetUserAsync(context.User.Id)).InternalId))
                : (await DBHelper.GetUsersPronounsAsync((await DBHelper.GetUserAsync(context.User.Id)).InternalId))
                    .Where(x => x.ToString().Contains(currentValue, StringComparison.InvariantCultureIgnoreCase))
                    .ToList()
			: currentValue == ""
                ? (await DBHelper.GetPronounsAsync())
                : (await DBHelper.GetPronounsAsync())
                    .Where(x => x.ToString().Contains(currentValue, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();

		pronouns = pronouns.Count > 20 
			? pronouns.GetRange(0, 20)
			: pronouns;

		return AutocompletionResult.FromSuccess(pronouns.Select(x => new AutocompleteResult(x.ToString("s/o/p/r"), 
			x.Id.ToString())));
	}

	protected override string GetLogString(IInteractionContext context) => nameof(PronounAutocompleteProvider);
}
