using System.Reflection;
using CatCore.Client.TypeConverters;
using System.Text.Json;
using System.Text.Json.Serialization;
using Discord.Rest;

namespace CatCore.Client.Commands;

internal class CommandHandler
{
	private readonly DiscordSocketClient _client;
	private readonly InteractionService _interactionService;
	private readonly IServiceProvider _services;
	private readonly Logger _logger;
	private readonly bool _debugMode;
	private readonly ulong _debugGuild;
	private bool _firstClientReady = true;

	public event Func<LogMessage, Task> Log;

	public CommandHandler(DiscordSocketClient client, InteractionService interaction, IServiceProvider service, 
		ClientSettings settings)
	{
		_logger = new("Command Handler", LogSeverity.Debug);
		_logger.LogFired += x => Log.Invoke(x);
		_client = client;
		_interactionService = interaction;
		_services = service;
		_debugMode = settings.DebugMode;
		_debugGuild = settings.DebugGuildId;
		client.Ready += ClientReady;
	}

	public async Task InitializeAsync()
	{
		try
		{	
			_logger.LogVerbose("Adding TypeConverters");
			_interactionService.AddGenericTypeConverter<Poll>(typeof(PollTypeConverter<>));
			_interactionService.AddGenericTypeConverter<Pronoun>(typeof(PronounTypeConverter<>));
			_interactionService.AddGenericTypeConverter<UserMessage>(typeof(UserMessageTypeConverter<>));
			_interactionService.AddGenericTypeConverter<MessageGroup>(typeof(MessageGroupTypeConverter<>));
			_interactionService.AddGenericTypeConverter<RegexAction>(typeof(RegexActionTypeConverter<>));

			await _interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), _services);

			_client.InteractionCreated += HandleInteraction;

			// TODO: post interaction cleanup and error logging.

			_interactionService.SlashCommandExecuted += SlashCommandExecuted;
		}
		catch (Exception ex)
		{
			_logger.LogCritical("Failed to start interactions service.", ex);
		}
	}

	private async Task SlashCommandExecuted(IApplicationCommandInfo command, IInteractionContext context, IResult result)
	{
		if (result.IsSuccess)
		{
			_logger.LogDebug($"Successfully executed the command {command.Name}");
			var unread = (context as CatCoreInteractionContext).DbUser.Messages
				.Where(x => x.IsNotifiable)
				.ToList();
			if (unread.Count >= 1)
			{
				var eb = new EmbedBuilder()
					.WithTitle("You have mail.")
					.WithColor(Color.Orange);
				if (unread.Count == 1)
				{
					eb.WithDescription("Run **`/mail inbox`** to view the very import message " +
						$"\"**{unread.First().Message.Title}**\".");
				}
				else
				{
					eb.WithDescription($"Run **`/mail inbox`** to view your **{unread.Count}** unread messages.");
					eb.AddField("Messages:", string.Concat(unread.Select(x => $"- **{x.Message.Title}**\n")));
				}

				var cb = new ComponentBuilder()
					.WithButton("Dismiss", $"user.notifications.dismiss", ButtonStyle.Danger);

				await context.Interaction.FollowupAsync(embed: eb.Build(), ephemeral: true, components: cb.Build());
			}
		}
		else
		{
			await InteractionFailHandle(command.Name, result, (CatCoreInteractionContext)context);
		}
	}

	private static async Task InteractionFailHandle(string name, IResult result, CatCoreInteractionContext context)
	{
		var interaction = context.Interaction;
		
		string error = $"The command {name} failed because of the {result.Error?.ToString() ?? "unknown"}" +
			$"\n```diff\n- {result?.ErrorReason.Replace("\n", "\n-")}\n```";
		// interaction timed out.
		if (!interaction.IsValidToken) return;

		if (!interaction.HasResponded)
		{
			await interaction.RespondAsync(error, ephemeral: true);
		}
		else
		{
			await interaction.FollowupAsync(error, ephemeral: true);
		}

		// Send additional information to developers
		if (context.DbUser.IsDev)
		{
			JsonSerializerOptions options = new()
			{
				WriteIndented = true,
				IgnoreReadOnlyFields = false,
				IgnoreReadOnlyProperties = false,
				ReferenceHandler = ReferenceHandler.IgnoreCycles,
			};
			string message = $"{JsonSerializer.Serialize(context.DbUser, options)}\n{JsonSerializer.Serialize((object)result, options)}";

			using MemoryStream ms = new();
			using StreamWriter sw = new(ms);
			sw.Write(message);
			sw.Flush();
			ms.Position = 0;
			await interaction.FollowupWithFileAsync(ms, "ErrorDetails.json", "Error details");
		}
	}

	private async Task HandleInteraction(SocketInteraction interaction)
	{
		var db = (CatCoreContext)_services.GetService(typeof(CatCoreContext));
		var context = new CatCoreInteractionContext(_client, interaction, db);
		await _interactionService.ExecuteCommandAsync(context, _services);
	}
	
	private async Task ClientReady()
	{
		// make the function only run once
		if (!_firstClientReady) return;
		_firstClientReady = false;
		
		RestGuild guild = await _client.Rest.GetGuildAsync(_debugGuild);
		if (!_debugMode)
		{
			// only regester sensitive commands to the debugging guild.
			var safeCommands = _interactionService.Modules
				.Where(x => !x.Attributes
					.Any(y => y is DebugModeCommandAttribute))
				.ToArray();
			var badCommands = _interactionService.Modules
				.Where(x => !safeCommands.Contains(x))
				.ToArray();
			await _interactionService.AddModulesGloballyAsync(true, safeCommands);
			await _interactionService.AddModulesToGuildAsync(guild, true, badCommands);
		}
		else await _interactionService.RegisterCommandsToGuildAsync(guild.Id);
	}
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class DebugModeCommandAttribute : Attribute
{
	
}
