using Discord.WebSocket;
using Discord.Interactions;
using Discord;
using System;
using System.Threading.Tasks;

namespace Client.TypeConverters
{
	// TODO: Replace with lib intigrated when functional.
	public class DefaultNullableValueConverter<T> : TypeConverter
	{
		public override ApplicationCommandOptionType GetDiscordType() => ApplicationCommandOptionType.Integer;

		public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IApplicationCommandInteractionDataOption option, IServiceProvider services)
		{
			object value;

			if (option.Value is Optional<object> optional)
				value = optional.IsSpecified ? optional.Value : default(T);
			else
				value = option.Value;

			try
			{
				var converted = Convert.ChangeType(value, Nullable.GetUnderlyingType(typeof(T)));
				return Task.FromResult(TypeConverterResult.FromSuccess(converted));
			}
			catch (InvalidCastException castEx)
			{
				return Task.FromResult(TypeConverterResult.FromError(castEx));
			}
		}

		public override void Write(ApplicationCommandOptionProperties properties, IParameterInfo parameter)
		{
			properties.IsRequired = false;
			properties.Type = ApplicationCommandOptionType.Integer;
		}

		public sealed override bool CanConvertTo(Type t)
		{
			Console.WriteLine("test");
			if (t == typeof(short?) || t == typeof(int?) || t == typeof(long?) ||
				t == typeof(ushort?) || t == typeof(uint?) || t == typeof(ulong?)) return true;
			else
			{
				Console.WriteLine(t);
				return false;
			}
		}
	}
}