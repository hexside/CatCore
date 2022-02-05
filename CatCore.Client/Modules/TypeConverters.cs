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
			var converted = ((CatCoreInteractionContext)context).DbUser.Messages
				.First(x => x.MessageId == int.Parse(value));
			return Task.FromResult(TypeConverterResult.FromSuccess(converted));
		}
		catch (Exception ex)
		{
			return Task.FromResult(TypeConverterResult.FromError(ex));
		}
	}
}
