using Discord.WebSocket;
using Discord.Interactions;
using Discord;
using System;
using System.Threading.Tasks;
using CatCore.Data;

namespace CatCore.Client.TypeConverters
{
	public class PollTypeConverter<T> : TypeConverter<T> where T : Poll
	{
		public DBHelper DBHelper { get; set; }
		public override ApplicationCommandOptionType GetDiscordType() => ApplicationCommandOptionType.String;

		public override async Task<TypeConverterResult> ReadAsync(IInteractionContext context, IApplicationCommandInteractionDataOption option, IServiceProvider services)
		{
			object value;

			if (option.Value is Optional<object> optional)
				value = optional.IsSpecified ? optional.Value : default(T);
			else
				value = option.Value;
			
			try
			{
				var converted = await DBHelper.GetPollAsync(Convert.ToUInt64(value.ToString()));
				return TypeConverterResult.FromSuccess(converted);
			}
			catch (InvalidCastException castEx)
			{
				return TypeConverterResult.FromError(castEx);
			}
		}
	}
}