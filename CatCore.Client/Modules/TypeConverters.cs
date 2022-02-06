using CatCore.Client.Commands;

namespace CatCore.Client.TypeConverters;

public class PollTypeConverter<T> : TypeConverter<T> where T : Poll
{
	public override ApplicationCommandOptionType GetDiscordType() => ApplicationCommandOptionType.String;

	public override async Task<TypeConverterResult> ReadAsync(IInteractionContext context,
		IApplicationCommandInteractionDataOption option, IServiceProvider services)
	{
		var db = (CatCoreContext)services.GetService(typeof(CatCoreContext));
		string value;

		if (option.Value is Optional<object> optional)
			value = optional.IsSpecified ? (string)optional.Value : "";
		else
			value = (string)option.Value;

		try
		{
			var converted = await db.Polls.FirstAsync(x => x.PollId == int.Parse(value));
			return TypeConverterResult.FromSuccess(converted);
		}
		catch (Exception ex)
		{
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
		var db = (CatCoreContext)services.GetService(typeof(CatCoreContext));
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
			return TypeConverterResult.FromError(ex);
		}
	}
}

public class UserMessageTypeConverter<T> : TypeConverter<T> where T : UserMessage
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
			int i = int.Parse(value);
			var converted = ((CatCoreInteractionContext)context).DbUser.Messages
				.First(x => x.UserMessageId == i);
			return Task.FromResult(TypeConverterResult.FromSuccess(converted));
		}
		catch (Exception ex)
		{
			return Task.FromResult(TypeConverterResult.FromError(ex));
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
