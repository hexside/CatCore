using CatCore.Client.Modals;
using CatCore.Client.Autocomplete;
using System.Text.RegularExpressions;

namespace CatCore.Client.Commands;

[Group("character", "character")]
public class CharacterCommands : InteractionModuleBase<CatCoreInteractionContext>
{
	[SlashCommand("create", "create a new character.")]
	public async Task Create
	(
		[Summary("avatar", "enter your character's profile picture.")]
		Attachment profile = null
	)
		=> await RespondWithModalAsync<CharacterModal>($"character.add${profile?.Url ?? ""};");

	[ModalInteraction("character.add$*;", true)]
	public async Task CreateModal(string profileUrl, CharacterModal modal)
	{
		Character character = new()
		{
			Creator = Context.DbUser,
			Description = modal.Description,
			ImageUrl = profileUrl,
			Name = modal.Name,
		};

		Context.DbGuild.Characters.Add(character);
		await Context.Db.SaveChangesAsync();
		await RespondAsync("Created your character", embed: character.GetEmbed().Build(), ephemeral: true);
	}

	[SlashCommand("update", "modify an already created character.")]
	public async Task Update
	(
		[Autocomplete(typeof(CharacterAutocompleteProvider))]
		[Summary("character", "What character do you want to update")]
		Character character,
		[Summary("avatar", "enter your character's profile picture.")]
		Attachment profile = null
	)
		=> await RespondWithModalAsync(new ModalBuilder()
			.WithTitle("Update A Character")
			.WithCustomId($"character.{character.CharacterId}.update${profile.Url};")
			.AddTextInput("Name", "name", TextInputStyle.Short, "Please enter your character's name.", 1, 30,
				value: character.Name)
			.AddTextInput("Description", "description", TextInputStyle.Paragraph, "Describe your character, markdown," +
				" links, and newlines are supported", 1, 500, value: character.Description)
			.Build());

	[ModalInteraction("character.*.update$*;", true)]
	public async Task UpdateModal(string characterIdStr, string? profileUrl, CharacterModal modal)
	{
		int characterId = int.Parse(characterIdStr);

		var character = Context.DbGuild.Characters.FirstOrDefault(x => x.CharacterId == characterId);

		if (character is null)
		{
			await RespondAsync($"The character **`{characterIdStr}`** is missing, deleted, or invalid. Please try again.",
				ephemeral: true);
			return;
		}

		character.Name = modal.Name;
		character.Description = modal.Description;
		character.ImageUrl = string.IsNullOrWhiteSpace(profileUrl)
			? character.ImageUrl
			: profileUrl;

		Context.Db.Characters.Update(character);
		await Context.Db.SaveChangesAsync();
		await RespondAsync("Updated your character", embed: character.GetEmbed().Build(), ephemeral: true);
	}

	[SlashCommand("color", "change the color of your characters embed")]
	public async Task UpdateColor
	(
		[Autocomplete(typeof(CharacterAutocompleteProvider))]
		[Summary("character", "The character to update")] Character character,
		[MaxValue(255)]
		[Summary("red", "The color's red value.")] int r,
		[MaxValue(255)]
		[Summary("green", "The color's green value.")] int g,
		[MaxValue(255)]
		[Summary("blue", "The color's blue value.")] int b
	)
	{
		character.Color = new Color(r, g, b).RawValue;
		Context.Db.Characters.Update(character);
		await Context.Db.SaveChangesAsync();
		await RespondAsync("Updated the characters color", embed: character.GetEmbed().Build(), ephemeral: true);
	}

	[SlashCommand("get", "Get data about a character")]
	public async Task Get
	(
		[Autocomplete(typeof(CharacterAutocompleteProvider))]
		[Summary("character", "The character to get information about")] Character character,
		[Summary("public", "Should the character's info be public")] bool isPublic = false
	)
		=> await RespondAsync(embed: character.GetEmbed().Build(), ephemeral: !isPublic);

	[SlashCommand("delete", "Delete a character.")]
	public async Task Delete
	(
		[Autocomplete(typeof(CharacterAutocompleteProvider))]
		[Summary("character", "The character to delete")] Character character
	)
		=> await RespondWithModalAsync(new ModalBuilder()
			.WithTitle("Confirm Character Deletion")
			.WithCustomId($"character.{character.CharacterId}.delete")
			.AddTextInput($"Type {character.Name}", "value", placeholder: character.Name)
			.Build());

	[Group("attribute", "attribute")]
	public class CharacterAttributeCommands : InteractionModuleBase<CatCoreInteractionContext>
	{
		[SlashCommand("add", "Add an attribute to your character")]
		public async Task Add
		(
			[Autocomplete(typeof(CharacterAutocompleteProvider))]
			[Summary("character", "The character to add an attribute to.")]Character character,
			[Autocomplete(typeof(GuildCharacterAttributeAutocompleteProvider))]
			[Summary("attribute", "The attribute to add to the character.")]GuildCharacterAttribute attribute
		)
		{
			if (character.Attributes.Any(x => x.BaseId == attribute.GuildCharacterAttributeId))
			{
				await Update(character, attribute);
				return;
			}

			var mb = new ModalBuilder()
				.WithTitle("Attribute Value")
				.WithCustomId($"character.{character.CharacterId}.attribute.{attribute.GuildCharacterAttributeId}.value")
				.AddTextInput(attribute.Name, "value", attribute.Multiline ?? true
					? TextInputStyle.Paragraph
					: TextInputStyle.Short, "Enter an attribute value", attribute.MinValue, attribute.MaxValue, true);


			await RespondWithModalAsync(mb.Build());
		}

		[SlashCommand("update", "modify an attribute on your character")]
		public async Task Update
		(
			[Autocomplete(typeof(CharacterAutocompleteProvider))]
			[Summary("character", "The character to add an attribute to.")]Character character,
			[Autocomplete(typeof(GuildCharacterAttributeAutocompleteProvider))]
			[Summary("attribute", "The attribute to add to the character.")]GuildCharacterAttribute attribute
		)
		{
			if (!character.Attributes.Any(x => x.BaseId == attribute.GuildCharacterAttributeId))
			{
				await Add(character, attribute);
				return;
			}

			string value = character.Attributes.First(x => x.BaseId == attribute.GuildCharacterAttributeId).Value;

			var mb = new ModalBuilder()
				.WithTitle("Attribute Value")
				.WithCustomId($"character.{character.CharacterId}.attribute.{attribute.GuildCharacterAttributeId}.value")
				.AddTextInput(attribute.Name, "value", attribute.Multiline ?? true
					? TextInputStyle.Paragraph
					: TextInputStyle.Short,
					"Enter an attribute value", attribute.MinValue, attribute.MaxValue, true, value);

			await RespondWithModalAsync(mb.Build());
		}

		[ModalInteraction("character.*.attribute.*.value", true)]
		public async Task AddModal(string characterIdStr, string attributeIdStr, AttributeValueModal modal)
		{
			int characterId = int.Parse(characterIdStr);
			int attributeId = int.Parse(attributeIdStr);
			var character = Context.DbGuild.Characters.First(x => x.CharacterId == characterId);
			var attribute = Context.DbGuild.GuildCharacterAttributes.First(x => x.GuildCharacterAttributeId == attributeId);

			if (!Regex.IsMatch(modal.Value, attribute.RegexValidator ?? ".*", 0, TimeSpan.FromMilliseconds(1)))
			{
				await RespondAsync($"**{attribute.Name}** must match **`{attribute.RegexValidator}`**", ephemeral: true);
				return;
			}

			character.Attributes.RemoveAll(x => x.BaseId == attributeId);
			character.Attributes.Add(new(attributeId, characterId, modal.Value));
			Context.Db.Characters.Update(character);
			await Context.Db.SaveChangesAsync();

			await RespondAsync("Updated the attribute.", embed: character.GetEmbed().Build(), ephemeral: true);
		}

		[SlashCommand("remove", "Remove an attribute form your character.")]
		public async Task Remove
		(
			[Autocomplete(typeof(CharacterAutocompleteProvider))]
			[Summary("character", "The character to remove the attribute from.")]Character character,
			[Autocomplete(typeof(GuildCharacterAttributeAutocompleteProvider))]
			[Summary("attribute", "The attribute to remove from the character.")]GuildCharacterAttribute attribute
		)
		{
			var characterAttribute = character.Attributes
				.FirstOrDefault(x => x.BaseId == attribute.GuildCharacterAttributeId);

			if (characterAttribute is null)
			{
				await RespondAsync("Your character doesn't have that attribute, run **`/character attribute add`** to" +
					" add it.", ephemeral: true);
				return;
			}

			if (characterAttribute.Base.Required == true)
			{
				await RespondAsync("You cannot remove required attributes from your character, run **`/character " +
					"attribute update`** to change the value.", ephemeral: true);
				return;
			}

			character.Attributes.Remove(characterAttribute);
			Context.Db.Characters.Update(character);
			await Context.Db.SaveChangesAsync();

			await RespondAsync("Removed the attribute", embed: character.GetEmbed().Build(), ephemeral: true);
		}
	}
}
