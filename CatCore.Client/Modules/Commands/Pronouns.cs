using CatCore.Client.Autocomplete;

namespace CatCore.Client.Commands;

// HACK: partial resolves duplication errors, group needs to be added.
// [Group("pronoun", "commands for managing pronouns,")]
public partial class PronounCommands : InteractionModuleBase<SocketInteractionContext>
{
	public CatCoreContext DB { get; set; }

	[SlashCommand("add", "add a pronoun to your profile")]
	public async Task Add
	(
		[Summary("pronoun", "the pronoun to add")]
		[Autocomplete(typeof(PronounAutocompleteProvider))]
		Pronoun pronoun
	)
	{
		User user = await DB.Users.FirstAsync(x => x.DiscordID == Context.User.Id);

		List<Pronoun> pronouns = user.Pronouns;
		
		if (pronouns.Any(x => x.PronounId == pronoun.PronounId))
		{
			await RespondAsync("You already have that pronoun", ephemeral: true);
			return;
		}

		user.Pronouns.Add(pronoun);
		DB.Users.Update(user);
		await DB.SaveChangesAsync();
		
		await RespondAsync($"added **{pronoun:s/o/p}** to your profile", ephemeral: true,
			allowedMentions: AllowedMentions.None);
	}

	[SlashCommand("get", "get another users pronouns")]
	public async Task Get(
	[Summary("user", "the user to get pronouns from")]
	SocketUser user)
		=> await Pronouns(user);
	
	//TODO: Convert to modal.
	[SlashCommand("new", "creates a new pronoun")]
	public async Task New(
	[Summary("subject", "They went to the park")]
	string subjective,
	[Summary("object", "I went to the park with them.")]
	string objective,
	[Summary("possessive", "The park is near their house.")]
	string possessiveAdj,
	[Summary("possessive", "That park is theirs.")]
	string possessivePnoun,
	[Summary("reflexive", "Someone went to the park by themself.")]
	string reflexive)
	{
		Pronoun pronoun = new()
		{
			Subject = subjective,
			Object = objective,
			PossessiveAdjective = possessiveAdj,
			PossessivePronoun = possessivePnoun,
			Reflexive = reflexive
		};

		if (DB.Pronouns.Any(x => x.Matches(pronoun)))
		{
			await RespondAsync("That pronoun already exists.", ephemeral: true);
			return;
		}

		await DB.Pronouns.AddAsync(pronoun);
		await DB.SaveChangesAsync();

		await RespondAsync($"created the pronoun **{pronoun:s/o/a/r}**", ephemeral: true);
	}

	[UserCommand("Pronouns")]
	public async Task Pronouns(SocketUser user)
	{
		List<Pronoun> pronouns = (await DB.Users.FirstAsync(x => x.DiscordID == user.Id)).Pronouns;

		string responce = pronouns.Count switch
		{
			0 => $"{user.Mention} did not specify their pronouns, have them run `/pronoun add` to set them",
			_ => $@"{user.Mention}'s pronouns are {string.Concat(pronouns.Select(x => $"**{x:s, o}**, "))
					[..^2].ReplaceLast(",", " and")}"
		};

		await Context.Interaction.RespondAsync(responce, ephemeral: true, allowedMentions: AllowedMentions.None);
	}

	[SlashCommand("remove", "Remove a pronoun to your profile.")]
	public async Task Remove(
		[Summary("remove", "the pronoun to remove")]
		[Autocomplete(typeof(PronounAutocompleteProvider))]
		[AutocompleteFromUserAtribute]
		Pronoun pronoun)
	{
		User user = await DB.Users.FirstAsync(x => x.DiscordID == Context.User.Id);

		user.Pronouns.Remove(pronoun);
		DB.Users.Update(user);
		await DB.SaveChangesAsync();
		
		await RespondAsync($"removed **{pronoun:s/o/a/r}** from your profile", ephemeral: true,
			allowedMentions: AllowedMentions.None);
	}

	[SlashCommand("try", "try out a pronoun on yourself")]
	public async Task TryPronoun(
		[Summary("pronoun", "the pronoun to try out")]
		[Autocomplete(typeof(PronounAutocompleteProvider))]
		Pronoun pronoun)
	{
		await RespondAsync($"Have you seen {Context.User.Mention}'s latest project? " +
			$"**{pronoun.Subject}** made a really pretty necklace that looks like **{pronoun.PossessiveAdjective}**" + 
			$"cats hugging. I wish I spent more time on projects like **{pronoun.Object}**.", ephemeral: true,
			allowedMentions: AllowedMentions.None);
	}
}
