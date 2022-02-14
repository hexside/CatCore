using MailSearchType = CatCore.Client.Commands.MailCommands.MailSearchType;

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
		IAutocompleteInteraction interaction, IParameterInfo parameter, IServiceProvider services)
	{
		string currentValue = interaction.Data.Current.Value.ToString();
		var polls = (context as CatCoreInteractionContext).DbGuild.Polls
			.Where(x => x.Title.Contains(currentValue, StringComparison.OrdinalIgnoreCase))
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

internal class UserMessageAutocompleteProvider : AutocompleteHandler
{
	public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context,
		IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
	{
		var db = (CatCoreContext)services.GetService(typeof(CatCoreContext));
		var searchType = (MailSearchType)Enum.Parse(typeof(MailSearchType), (string)autocompleteInteraction.Data.Options
			.First(x => x.Name == "filter")
			.Value);
		var mail = (await db.Users
			.Include(x => x.Messages)
			.ThenInclude(x => x.Message)
			.FirstAsync(x => x.DiscordID == context.User.Id))
			.Messages;
		string currentValue = autocompleteInteraction.Data.Current.Value.ToString();

		var resolved = mail
			.Where(x => (x.Message.Title + x.Message.Description)
				.Contains(currentValue, StringComparison.OrdinalIgnoreCase))
			.Where(x => searchType != MailSearchType.Read || x.IsRead)
			.Where(x => searchType != MailSearchType.Unread || !x.IsRead)
			.RangeOrDefault(0, 20)
			.Select(x => new AutocompleteResult(x.Message.Title, x.UserMessageId.ToString()));

		return AutocompletionResult.FromSuccess(resolved);
	}
}

internal class MessageGroupAutocompleteProvider : AutocompleteHandler
{
	public override async Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context,
		IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
	{
		bool fromUser = parameter.Attributes.Any(x => x is AutocompleteFromUserAtribute);
		var db = (CatCoreContext)services.GetService(typeof(CatCoreContext));
		var user = ((CatCoreInteractionContext)context).DbUser;
		string currentValue = autocompleteInteraction.Data.Current.Value.ToString();
		
		var groups = (await db.MessageGroups
			.Include(x => x.VisiableTo)
			.Where(x => x.IsPublic || x.VisiableTo.Contains(user) || user.IsDev)
			.Where(x => !fromUser || user.MessageGroups.Contains(x))
			.ToListAsync())
			.Where(x => x.Name.Contains(currentValue, StringComparison.OrdinalIgnoreCase))
			.RangeOrDefault(0, 20)
			.Select(x => new AutocompleteResult(x.Name, x.MessageGroupId.ToString()));

		return AutocompletionResult.FromSuccess(groups);
	}
}

internal class RegexActionAutocompleteProvider : AutocompleteHandler
{
	public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context,
		IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
	{
		var dbGuild = (context as CatCoreInteractionContext).DbGuild;
		string currentValue = autocompleteInteraction.Data.Current.Value.ToString();

		var regexActions = dbGuild.RegexActions
			.Where(x => x.Valid)
			.Where(x => x.ActionName.Contains(currentValue, StringComparison.OrdinalIgnoreCase))
			.Select(x => new AutocompleteResult(x.ActionName, x.RegexActionId));

		return Task.FromResult(AutocompletionResult.FromSuccess(regexActions));
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
