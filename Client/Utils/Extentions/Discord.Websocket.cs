using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket;

public static partial class WebSocketExtensions
{
	public static string GetCommandName(this SocketSlashCommand command)
	{
		// test
		string commandName = command.Data.Name + ".";
		// test.group
		commandName += command.Data.Options?.Where(x =>
			x.Type is ApplicationCommandOptionType.SubCommandGroup or ApplicationCommandOptionType.SubCommand)
			.Select(x => x.Name).FirstOrDefault() + ".";
		// test.group.first
		commandName += command.Data.Options?.FirstOrDefault()?.Options?.Where(x =>
			x.Type == ApplicationCommandOptionType.SubCommand).Select(x => x.Name)
			.FirstOrDefault() + ".";

		while (commandName.EndsWith(".")) commandName = commandName[0..^1];
		return commandName;
	}

	public static Dictionary<string, object>? GetResolvedOptions(this SocketSlashCommand command)
	{
		if (command.Data.Options is null || command.Data.Options?.Count == 0) return null;
		Dictionary<string, object> options = new();
		List<SocketSlashCommandDataOption> badOptions = command.Data.Options.ToList();
		
		int depth = command.GetCommandName().Where(x => x == '.').Count();
		for (int i = 0; i < depth; i++)
		{
			badOptions = badOptions[0].Options?.ToList();
			if (badOptions is null || badOptions.Count == 0) return null;
		}

		badOptions.OnEach(x => options.Add(x.Name, x.Value));

		return options;
	}
}