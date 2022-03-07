using CatCore.Client.Commands;

namespace CatCore.Client.TypeConverters;

public class PollTypeConverter<T> : TypeConverter<T> where T : Poll
{
	public override ApplicationCommandOptionType GetDiscordType() => ApplicationCommandOptionType.String;

	public override async Task<TypeConverterResult> ReadAsync(IInteractionContext context,
		IApplicationCommandInteractionDataOption option, IServiceProvider services)
	{
		var catCoreContext = (CatCoreInteractionContext)context;
		string value;

		if (option.Value is Optional<object> optional)
			value = optional.IsSpecified ? (string)optional.Value : "";
		else
			value = (string)option.Value;

		try
		{
			int iValue = int.Parse(value);
			var converted = catCoreContext.DbGuild.Polls.First(x => x.PollId == iValue);
			return TypeConverterResult.FromSuccess(converted);
		}
		catch (Exception ex)
		{
			await context.Interaction.RespondAsync("Oops, we couldn't load that poll, make sure you picked something from the autocomplete menu.", ephemeral: true);
			return TypeConverterResult.FromError(ex);
		}
	}
}

public class PronounTypeConverter<T> : TypeConverter<T> where T : Pronoun
{
	public override ApplicationCommandOptionType GetDiscordType()
		=> ApplicationCommandOptionType.String;

	public override async Task<TypeConverterResult> ReadAsync(IInteractionContext context,
		IApplicationCommandInteractionDataOption option, IServiceProvider services)
	{
		var db = (CatCoreDbContext)services.GetService(typeof(CatCoreDbContext));
		string value;

		if (option.Value is Optional<object> optional)
			value = optional.IsSpecified ? (string)optional.Value : "";
		else
			value = (string)option.Value;

		try
		{
			var converted = await db.Pronouns.FirstAsync(x => x.PronounId == int.Parse(value));
			return TypeConverterResult.FromSuccess(converted);
		}
		catch (Exception ex)
		{
			await context.Interaction.RespondAsync("Oops, that pronoun isn't in the database, run `/pronoun new` to add it.", ephemeral: true);
			return TypeConverterResult.FromError(ex);
		}
	}
}

public class UserMessageTypeConverter<T> : TypeConverter<T> where T : UserMessage
{
	public override ApplicationCommandOptionType GetDiscordType()
		=> ApplicationCommandOptionType.String;

	public override async Task<TypeConverterResult> ReadAsync(IInteractionContext context, IApplicationCommandInteractionDataOption option, IServiceProvider services)
	{
		string value;
		if (option.Value is Optional<object> optional)
			value = optional.IsSpecified ? (string)optional.Value : "";
		else
			value = (string)option.Value;

		try
		{
			int i = int.Parse(value);
			var converted = ((CatCoreInteractionContext)context).DbUser.Messages
				.First(x => x.UserMessageId == i);
			return TypeConverterResult.FromSuccess(converted);
		}
		catch (Exception ex)
		{
			await context.Interaction.RespondAsync("Something went wrong loading that message.", ephemeral: true);
			return TypeConverterResult.FromError(ex);
		}
	}
}

public class MessageGroupTypeConverter<T> : TypeConverter<T> where T : MessageGroup
{
	public override ApplicationCommandOptionType GetDiscordType()
		=> ApplicationCommandOptionType.String;

	public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IApplicationCommandInteractionDataOption option, IServiceProvider services)
	{
		string value;
		if (option.Value is Optional<object> optional)
			value = optional.IsSpecified ? (string)optional.Value : "";
		else
			value = (string)option.Value;

		try
		{
			var converted = ((CatCoreInteractionContext)context).Db.MessageGroups
				.Include(x => x.Messages)
				.Include(x => x.VisiableTo)
				.First(x => x.MessageGroupId == int.Parse(value));
			return Task.FromResult(TypeConverterResult.FromSuccess(converted));
		}
		catch (Exception ex)
		{
			return Task.FromResult(TypeConverterResult.FromError(ex));
		}
	}
}

public class RegexActionTypeConverter<T> : TypeConverter<T> where T : RegexAction
{
	public override ApplicationCommandOptionType GetDiscordType()
		=> ApplicationCommandOptionType.String;

	public override async Task<TypeConverterResult> ReadAsync(IInteractionContext context, IApplicationCommandInteractionDataOption option, IServiceProvider services)
	{
		string value;
		if (option.Value is Optional<object> optional)
			value = optional.IsSpecified ? (string)optional.Value : "";
		else
			value = (string)option.Value;

		try
		{
			var intValue = int.Parse(value);
			var converted = ((CatCoreInteractionContext)context).DbGuild.RegexActions
				.First(x => x.RegexActionId == intValue);
			return TypeConverterResult.FromSuccess(converted);
		}
		catch (Exception ex)
		{
			await context.Interaction.RespondAsync("We couldn't load that automod action, make sure you picked something from the autocomplete menu.", ephemeral: true);
			return TypeConverterResult.FromError(ex);
		}
	}
}

public class CharacterTypeConverter<T> : TypeConverter<T> where T : Character
{
	public override ApplicationCommandOptionType GetDiscordType()
		=> ApplicationCommandOptionType.String;

	public override async Task<TypeConverterResult> ReadAsync(IInteractionContext context, IApplicationCommandInteractionDataOption option, IServiceProvider services)
	{
		string value;
		if (option.Value is Optional<object> optional)
			value = optional.IsSpecified ? (string)optional.Value : "";
		else
			value = (string)option.Value;

		try
		{
			var intValue = int.Parse(value);
			var converted = ((CatCoreInteractionContext)context).DbGuild.Characters
				.First(x => x.CharacterId == intValue);
			return TypeConverterResult.FromSuccess(converted);
		}
		catch (Exception ex)
		{
			await context.Interaction.RespondAsync("We couldn't load that character, make sure you picked something from the autocomplete menu.", ephemeral: true);
			return TypeConverterResult.FromError(ex);
		}
	}
}

public class GuildCharacterAttributeTypeConverter<T> : TypeConverter<T> where T : GuildCharacterAttribute
{
	public override ApplicationCommandOptionType GetDiscordType()
		=> ApplicationCommandOptionType.String;

	public override async Task<TypeConverterResult> ReadAsync(IInteractionContext context, IApplicationCommandInteractionDataOption option, IServiceProvider services)
	{
		string value;
		if (option.Value is Optional<object> optional)
			value = optional.IsSpecified ? (string)optional.Value : "";
		else
			value = (string)option.Value;

		try
		{
			var intValue = int.Parse(value);
			var converted = ((CatCoreInteractionContext)context).DbGuild.GuildCharacterAttributes
				.First(x => x.GuildCharacterAttributeId == intValue);
			return TypeConverterResult.FromSuccess(converted);
		}
		catch (Exception ex)
		{
			await context.Interaction.RespondAsync("We couldn't load that guild character attribute, make sure you picked something from the autocomplete menu.", ephemeral: true);
			return TypeConverterResult.FromError(ex);
		}
	}
}
