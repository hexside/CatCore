using System;
using System.Drawing;
using System.Linq;

namespace Discord;

public static partial class DiscordExtensions
{
	public static SlashCommandOptionBuilder ValidateNonCommand(this SlashCommandOptionBuilder builder) =>
		builder.Type is not ApplicationCommandOptionType.SubCommand and not ApplicationCommandOptionType.SubCommandGroup
			? builder
			: throw new ArgumentException($"Command option must not be of type SubCommand or SubCommandGroup.");

	public static string ToFormattedString(this LogMessage message)
		=> $"[{message.Severity.GetVtsFormat()}{message.Severity.GetShortName()}\x1b[m" +
		$":\x1b[34m {DateTime.Now.ToString($"{DateTime.Now.DayOfYear,3}:HH:mm:ss:f")} \x1b[m] " +
		$"{string.Concat(message.Source.Take(14)),-15} {message.Message} {message.Exception?.ToString()}";


	public static string GetShortName(this LogSeverity severity) => severity switch
	{
		LogSeverity.Critical => "crt",
		LogSeverity.Error => "err",
		LogSeverity.Warning => "wrn",
		LogSeverity.Info => "inf",
		LogSeverity.Verbose => "vrb",
		LogSeverity.Debug => "dbg",
		_ => throw new ArgumentException("severity must be a valid LogSeverity.")
	};

	public static Color GetColor(this LogSeverity severity) => severity switch
	{
		LogSeverity.Critical => Color.DarkRed,
		LogSeverity.Error => Color.Red,
		LogSeverity.Warning => Color.Orange,
		LogSeverity.Info => (Color)System.Drawing.Color.White,
		LogSeverity.Verbose => Color.LightGrey,
		LogSeverity.Debug => Color.LighterGrey,
		_ => throw new ArgumentException("severity must be a valid LogSeverity.")
	};

	public static string GetVtsFormat(this LogSeverity severity) => severity switch
	{
		LogSeverity.Critical => "\x1b[31m",
		LogSeverity.Error => "\x1b[91m",
		LogSeverity.Warning => "\x1b[33m",
		LogSeverity.Info => "\x1b[32m\x1b[1m",
		LogSeverity.Verbose => "\x1b[37m",
		LogSeverity.Debug => "\x1b[90m",
		_ => "\x1b[45m"
	};
}
