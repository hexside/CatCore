using System.Text.RegularExpressions;
using CatCore.Client.Autocomplete;
using CatCore.Client.Modals;

namespace CatCore.Client.Commands;

[Group("admin", "guild")]
[RequireBotPermission(GuildPermission.ManageMessages | GuildPermission.SendMessages)]
public class AdminCommands : InteractionModuleBase<CatCoreInteractionContext>
{
	[Group("automod", "automod")]
	public class AdminAutomodCommands : InteractionModuleBase<CatCoreInteractionContext>
	{
		[SlashCommand("add", "Adds a new filter.")]
		[RequireUserPermission(GuildPermission.ManageMessages)]
		public async Task AddFilter
		(
			[Summary("action", "What should happen when a message is flagged.")] RegexActionType type,
			[Summary("regex", "Is this a regex filter? (leave blank for all but advanced usecases)")]
				bool useRegex = false,
			[Summary("clean-message", "Should formatting and alternate fonts be converted to normal text?")]
				bool cleanMessage = true,
			[Summary("remove-spaces", "Should spaces, newlines, and other whitespace be removed before evaluating the filter?")]
				bool removeSpaces = false
		)
		{
			if (Context.DbGuild.MessageFlagChannelId is 0)
			{
				await RespondAsync("A valid message flag channel is required to run this command.", ephemeral: true);
				return;
			}

			RegexAction action = new()
			{
				CleanMessage = cleanMessage,
				RemoveWhitespace = removeSpaces,
				Type = type,
				Valid = false,
			};
			Context.DbGuild.RegexActions.Add(action);
			await Context.Db.SaveChangesAsync();
			if (useRegex) await RespondWithModalAsync<AutomodRegexInputModal>($"automod.filter.{action.RegexActionId}" +
				$".setRegex:0;");
			else await RespondWithModalAsync<AutomodInputModal>($"automod.filter.{action.RegexActionId}" +
				$".setRegex:1;");
		}

		[SlashCommand("update", "updates an already created filter.")]
		[RequireUserPermission(GuildPermission.ManageMessages)]
		public async Task UpdateFilter
		(
			[Autocomplete(typeof(RegexActionAutocompleteProvider))]
			[Summary("filter", "What filter would you like to update.")] RegexAction action,
			[Summary("action", "What should happen when a message is flagged.")] RegexActionType? type = null,
			[Summary("regex", "Is this a regex filter? (leave blank for all but advanced usecases)")]
				bool useRegex = false,
			[Summary("clean-message", "Should formatting and alternate fonts be converted to normal text?")]
				bool? cleanMessage = null,
			[Summary("remove-spaces", "Should spaces, newlines, and other whitespace be removed before evaluating the filter?")]
				bool? removeSpaces = null
		)
		{
			if (Context.DbGuild.MessageFlagChannelId is 0)
			{
				await RespondAsync("A valid message flag channel is required to run this command.", ephemeral: true);
				return;

			}

			action.Type = type ?? action.Type;
			action.CleanMessage = cleanMessage ?? action.CleanMessage;
			action.RemoveWhitespace = removeSpaces ?? action.RemoveWhitespace;

			await Context.Db.SaveChangesAsync();
			if (useRegex) await RespondWithModalAsync<AutomodRegexInputModal>($"automod.filter.{action.RegexActionId}" +
				$".setRegex:0;");
			else await RespondWithModalAsync<AutomodInputModal>($"automod.filter.{action.RegexActionId}" +
				$".setRegex:1;");
		}

		[ModalInteraction("automod.filter.*.setRegex:*;", true)]
		[RequireUserPermission(GuildPermission.ManageMessages)]
		public async Task AddRegexFilterModal(string idStr, string escapeRegexStr, AutomodRegexInputModal modal)
			=> await AddFilterModal(idStr, escapeRegexStr, new() { Name = modal.Name, Filter = modal.Filter });
		[RequireUserPermission(GuildPermission.ManageMessages)]
		public async Task AddFilterModal(string idStr, string escapeRegexStr, AutomodInputModal modal)
		{
			if (Context.DbGuild.MessageFlagChannelId is 0)
			{
				await RespondAsync("A valid message flag channel is required to run this command.", ephemeral: true);
				return;
			}

			int id = int.Parse(idStr); var action = Context.DbGuild.RegexActions.First(x => x.RegexActionId == id);

			bool escapeRegex = int.Parse(escapeRegexStr).GetBool();
			bool firstUse = !action.Valid;

			action.RegexString = escapeRegex
				? Regex.Escape(modal.Filter)
				: modal.Filter;
			action.Valid = true;
			action.ActionName = modal.Name;

			await Context.Db.SaveChangesAsync();
			if (firstUse)
				await RespondAsync($"Created the filter **{modal.Name}** run `/automod test` to make sure it works.",
					ephemeral: true);
			else
				await RespondAsync($"Updated **{modal.Name}** run `/automod filter test` to make sure it works",
					ephemeral: true);
		}

		[SlashCommand("test", "Test an automod filter.")]
		[RequireUserPermission(GuildPermission.ManageMessages)]
		public async Task UpdateFilter()
			=> await RespondWithModalAsync<AutomodTestModal>("automod.filter.test");

		[ModalInteraction("automod.filter.test", true)]
		public async Task AutomodFilterTest(AutomodTestModal modal)
		{
			List<Embed> embeds = new();
			Context.DbGuild.RegexActions
				.Where(x => x.ShouldExecute(modal.Message))
				.OnEach(x => embeds.Add(new EmbedBuilder()
					.WithTitle("Action Triggered")
					.WithDescription($"{x.ActionName} : `{x.RegexString}`")
					.Build()));

			await RespondAsync($"**{embeds.Count}** filters triggered", embeds: embeds.ToArray(), ephemeral: true);
		}

		[SlashCommand("remove", "remove an automod filter.")]
		public async Task RemoveAutomodFilter
		(
			[Autocomplete(typeof(RegexActionAutocompleteProvider))]
			[Summary(null, "The filter to remove")] RegexAction filter
		)
		{
			Context.DbGuild.RegexActions.Remove(filter);
			await Context.Db.SaveChangesAsync();

			await RespondAsync("Deleted the filter.", ephemeral: true);
		}

		[SlashCommand("notifications", "Change the notification channel.")]
		public async Task AutomodNotificationChannel
		(
			[Summary("channel", "What channel should automod notifications be sent to?")] ITextChannel channel
		)
		{
			Context.DbGuild.MessageFlagChannelId = channel.Id;
			await Context.Db.SaveChangesAsync();
			await RespondAsync("Updated the automod channel.", ephemeral: true);
		}
	}

	[Group("attribute", "attribute")]
	[RequireUserPermission(GuildPermission.ManageGuild)]
	public class AdminAttributeCommands : InteractionModuleBase<CatCoreInteractionContext>
	{
		[SlashCommand("add", "add a new attribute")]
		public async Task Add
		(
			[Summary("required", "Do characters need the attribute to rollplay?")] bool required = false,
			[Summary("validator", "Enter the regex validator to check attributes against")] string validator = ".*",
			[Summary("min-length", "Enter the shortest aloud input length")] int? minValue = null,
			[Summary("max-length", "Enter the largest aloud input length")] int? maxValue = null,
			[Summary("multiline", "Is the attribute multiline?")] bool multiline = false
		)
		{
			GuildCharacterAttribute attribute = new()
			{
				Required = required,
				RegexValidator = validator,
				MinValue = minValue,
				MaxValue = maxValue,
				Multiline = multiline,
				Name = "",
				Description = "",
				Valid = false,
			};

			Context.DbGuild.GuildCharacterAttributes.Add(attribute);
			await Context.Db.SaveChangesAsync();

			await RespondWithModalAsync<GuildCharacterAttributeModal>
				($"attribute.{attribute.GuildCharacterAttributeId}.update");
		}

		[ModalInteraction("attribute.*.update", true)]
		public async Task EditModal(string idStr, GuildCharacterAttributeModal modal)
		{
			int id = int.Parse(idStr);
			GuildCharacterAttribute attribute = Context.DbGuild.GuildCharacterAttributes
				.First(x => x.GuildCharacterAttributeId == id);

			attribute.Valid = true;
			attribute.Description = modal.Description;
			attribute.Name = modal.Name;
			Context.Db.GuildCharacterAttributes.Update(attribute);
			await Context.Db.SaveChangesAsync();

			await RespondAsync("update the attribute", ephemeral: true);
		}

		[SlashCommand("edit", "modify a guild attribute")]
		public async Task Edit
		(
			[Autocomplete(typeof(GuildCharacterAttributeAutocompleteProvider))]
			[Summary("attribute", "the attribute to edit.")] GuildCharacterAttribute attribute,
			[Summary("required", "Do characters need the attribute to rollplay?")] bool? required = null,
			[Summary("validator", "Enter the regex validator to check attributes against")] string? validator = null,
			[Summary("min-length", "Enter the shortest aloud input length")] int? minValue = null,
			[Summary("max-length", "Enter the largest aloud input length")] int? maxValue = null,
			[Summary("multiline", "Is the attribute multiline?")] bool? multiline = null
		)
		{
			attribute.Required = required ?? attribute.Required;
			attribute.RegexValidator = validator ?? attribute.RegexValidator;
			attribute.MinValue = minValue ?? attribute.MinValue;
			attribute.MaxValue = maxValue ?? attribute.MaxValue;
			attribute.Multiline = multiline ?? attribute.Multiline;

			Context.Db.GuildCharacterAttributes.Update(attribute);
			await Context.Db.SaveChangesAsync();

			var mb = new ModalBuilder()
				.WithCustomId($"attribute.{attribute.GuildCharacterAttributeId}.update")
				.WithTitle("Update attribute.")
				.AddTextInput("name", "name", TextInputStyle.Short, attribute.Name, 1, 30)
				.AddTextInput("description", "description", TextInputStyle.Short, attribute.Description, 1, 80);

			await RespondWithModalAsync(mb.Build());
		}

		[SlashCommand("delete", "delete a character attribute")]
		public async Task Delete
		(
			[Autocomplete(typeof(GuildCharacterAttributeAutocompleteProvider))]
			[Summary("attribute", "the attribute to delete.")] GuildCharacterAttribute attribute
		)
		{
			Context.DbGuild.GuildCharacterAttributes.Remove(attribute);
			await Context.Db.SaveChangesAsync();
			await RespondAsync("deleted the attribute", ephemeral: true);
		}
	}
}
