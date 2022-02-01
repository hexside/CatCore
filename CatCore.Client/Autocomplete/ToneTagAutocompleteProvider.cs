using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using CatCore.Data;

namespace CatCore.Client.Autocomplete;
internal class ToneTagAutocompleteProvider : AutocompleteHandler
{
	public DBHelper DBHelper { get; set; }
	public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context,
		IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
	{
		var tags = (List<ToneTag>)services.GetService(typeof(List<ToneTag>));
		string currentValue = autocompleteInteraction.Data.Current.Value.ToString();

		var resolvedTags = tags
			.Where(x => x.AllValues.Any(x => x.Contains(currentValue, StringComparison.InvariantCultureIgnoreCase)))
			.RemoveDuplicates(x => x.DefaultName)
			.RangeOrDefault(0, 20);

		return Task.FromResult(AutocompletionResult.FromSuccess(resolvedTags.Select(x => 
			new AutocompleteResult(x.DefaultName, x.DefaultName))));
	}
}
