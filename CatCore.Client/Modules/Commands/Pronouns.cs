using CatCore.Client.Modals;
using CatCore.Client.Autocomplete;

namespace CatCore.Client.Commands;

[Group("pronoun", "commands for managing pronouns,")]
public class PronounCommands : InteractionModuleBase<CatCoreInteractionContext>
{
	[SlashCommand("add", "Add a pronoun to your profile.")]
	public async Task Add
	(
		[Summary("pronoun", "The pronoun to add.")]
		[Autocomplete(typeof(PronounAutocompleteProvider))] Pronoun pronoun
	)
	{
		User user = await Context.Db.Users.FirstAsync(x => x.DiscordID == Context.User.Id);

		List<Pronoun> pronouns = user.Pronouns;

		if (pronouns.Any(x => x.PronounId == pronoun.PronounId))
		{
			await RespondAsync("You already have that pronoun", ephemeral: true);
			return;
		}

		user.Pronouns.Add(pronoun);
		Context.Db.Users.Update(user);
		await Context.Db.SaveChangesAsync();

		await RespondAsync($"added **{pronoun:s/o/p}** to your profile", ephemeral: true,
			allowedMentions: AllowedMentions.None);
	}

	[SlashCommand("get", "Get another users pronouns.")]
	public async Task Get(
	[Summary("user", "The user to get pronouns from.")]
	SocketUser user)
		=> await Pronouns(user);

	[SlashCommand("new", "Create a new pronoun.")]
	public async Task PronounNew() => await RespondWithModalAsync<PronounModal>("pronoun.new");

	[ModalInteraction("pronoun.new", true)]
	public async Task New(PronounModal modal)
	{
		Pronoun pronoun = new()
		{
			Subject = modal.Subjective,
			Object = modal.Objective,
			PossessiveAdjective = modal.PossessiveAdjective,
			PossessivePronoun = modal.PossessivePronoun,
			Reflexive = modal.Reflexive
		};

		if ((await Context.Db.Pronouns.ToListAsync()).Any(x => x.Matches(pronoun)))
		{
			await RespondAsync("That pronoun already exists.", ephemeral: true);
			return;
		}

		await Context.Db.Pronouns.AddAsync(pronoun);
		await Context.Db.SaveChangesAsync();

		await RespondAsync($"Created the pronoun **{pronoun:s/o/a/r}**.", ephemeral: true);
	}

	[UserCommand("Pronouns")]
	public async Task Pronouns(SocketUser user)
	{
		User? dbUser = await Context.Db.Users.FirstOrDefaultAsync(x => x.DiscordID == user.Id);
		List<Pronoun> pronouns = dbUser?.Pronouns ?? new();

		if (dbUser?.AnyPronouns ?? false)
		{
			await RespondAsync($"{user.Mention} uses any pronouns.", allowedMentions: AllowedMentions.None,
				ephemeral: true);
			return;
		}
		if (dbUser?.NoPronouns ?? false)
		{
			await RespondAsync($"{user.Mention} doesn't have any pronouns.", allowedMentions: AllowedMentions.None,
				ephemeral: true);
			return;
		}

		if (pronouns is null || pronouns.Count < 1)
		{
			await RespondAsync($"{user.Mention} did not specify their pronouns, have them run **`/pronoun add`** to set them.",
				ephemeral: true, allowedMentions: AllowedMentions.None);
			return;
		}
		string pronounsStr = string.Concat(pronouns.Select(x => $"{x.ToString("**s**, **o**")}, "))
			[..^2].ReplaceLast(",", " and");

		await RespondAsync($@"{user.Mention}'s pronouns are {pronounsStr}", ephemeral: true,
			allowedMentions: AllowedMentions.None);
	}

	[SlashCommand("remove", "Remove a pronoun to your profile.")]
	public async Task Remove
	(
		[Autocomplete(typeof(PronounAutocompleteProvider))]
		[AutocompleteFromUserAtribute]
		[Summary("pronoun", "The pronoun to remove.")] Pronoun pronoun
	)
	{
		Context.DbUser.Pronouns.Remove(pronoun);
		Context.Db.Users.Update(Context.DbUser);
		await Context.Db.SaveChangesAsync();

		await RespondAsync($"Removed {pronoun:**s**/**o**/**a**} from your profile.", ephemeral: true,
			allowedMentions: AllowedMentions.None);
	}

	[SlashCommand("try", "Try out a pronoun on yourself.")]
	public async Task TryPronoun(
		[Summary("pronoun", "The pronoun to try out.")]
		[Autocomplete(typeof(PronounAutocompleteProvider))]
		Pronoun pronoun)
	{
		await RespondAsync($"Have you seen {Context.User.Mention}'s latest project? " +
			$"**{pronoun.Subject}** made a really pretty necklace that looks like **{pronoun.PossessiveAdjective}** " +
			$"cats hugging. I wish I spent more time on projects like **{pronoun.Object}**.", ephemeral: true,
			allowedMentions: AllowedMentions.None);
	}

	[SlashCommand("any", "Set your profile to use any pronoun.")]
	public async Task AnyPronounsAsync
	(
		[Summary("enable", "Should your profile say you use any pronouns?")] bool? enable = null
	)
	{
		if (enable is null)
		{
			await RespondAsync(Context.DbUser.AnyPronouns
				? "You currently have any pronouns set"
				: "You are using sets of pronouns.", ephemeral: true);
			return;
		}
		else Context.DbUser.AnyPronouns = enable.Value;
	}

	[SlashCommand("none", "Mark you as not wanting to us pronouns.")]
	public async Task NoPronounsAsync
	(
		[Summary("enable", "Should your profile not have any pronouns?")] bool? enable = null
	)
	{
		if (enable is null)
		{
			await RespondAsync(Context.DbUser.AnyPronouns
				? "You currently have any pronouns set"
				: "You are using sets of pronouns.", ephemeral: true);
			return;
		}
		else Context.DbUser.AnyPronouns = enable.Value;
	}
}
