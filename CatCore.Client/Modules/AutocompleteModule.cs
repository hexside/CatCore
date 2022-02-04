
namespace CatCore.Client.Autocomplete;
internal class PronounAutocompleteProvider : AutocompleteHandler
{
	public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context,
		IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
	{
		bool fromUser = parameter.Attributes.Any(x => x is AutocompleteFromUserAtribute);
		string currentValue = autocompleteInteraction.Data.Current.Value.ToString();
		var db = (CatCoreContext)services.GetService(typeof(CatCoreContext));

		User user = await db.Users.Include(x => x.Pronouns).FirstAsync(x => x.DiscordID == context.User.Id);

		var pronouns = (fromUser ? user.Pronouns : await db.Pronouns.ToListAsync())
			.Where(x => x.ToString("s/o/a/p/r").Contains(currentValue, StringComparison.OrdinalIgnoreCase))
			.ToList();

		pronouns = pronouns.Count > 20
			? pronouns.GetRange(0, 20)
			: pronouns;

		return AutocompletionResult.FromSuccess(pronouns.Select(x => new AutocompleteResult(x.ToString("s/o/a/r"),
			x.PronounId.ToString())));
	}

	protected override string GetLogString(IInteractionContext context) => nameof(PronounAutocompleteProvider);
}

internal class PollAutocompleteProvider : AutocompleteHandler
{
	public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context,
		IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
	{
		string currentValue = autocompleteInteraction.Data.Current.Value.ToString();
		var db = (CatCoreContext)services.GetService(typeof(CatCoreContext));

		var polls = db.Polls
			.Where(x => x.ToString(true).Contains(currentValue, StringComparison.InvariantCultureIgnoreCase))
			.OrderBy(x => x.PollId)
			.ToList();

		polls = polls.Count > 20
			? polls.GetRange(0, 20)
			: polls;

		return Task.FromResult(AutocompletionResult
			.FromSuccess(polls.Select(x => new AutocompleteResult(x.ToString(false), x.PollId.ToString()))));
	}
}

internal class ToneTagAutocompleteProvider : AutocompleteHandler
{
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


/// <summary>
/// This attribute makes supported autocomplete providers
/// autocomplete from the command's user's values instead of
/// the entire db.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
public class AutocompleteFromUserAtribute : Attribute
{
	public bool Set = false;
	public AutocompleteFromUserAtribute() => Set = true;
}
