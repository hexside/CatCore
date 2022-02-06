using System.Reflection;
using CatCore.Client.TypeConverters;

namespace CatCore.Client.Commands;

internal class CommandHandler
{
	private readonly DiscordSocketClient _client;
	private readonly InteractionService _interactionService;
	private readonly IServiceProvider _services;
	private readonly Logger _logger;

	public event Func<LogMessage, Task> Log;

	public CommandHandler(DiscordSocketClient client, InteractionService interaction, IServiceProvider service)
	{
		_logger = new("Command Handler", LogSeverity.Debug);
		_logger.LogFired += x => Log.Invoke(x);
		_client = client;
		_interactionService = interaction;
		_services = service;
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
			await InteractionFailHandle(command.Name, result, context.Interaction as SocketInteraction);
		}
	}

	private static async Task InteractionFailHandle(string name, IResult result, SocketInteraction interaction)
	{
		string error = $"The command {name} failed because of the {result.Error?.ToString() ?? "unknown"} error \n```\n{result?.ErrorReason}\n```";
		// interaction timed out.
		if (!interaction.IsValidToken) return;

		if (interaction.HasResponded)
		{
			await interaction.RespondAsync(error, ephemeral: true);
		}
		else
		{
			await interaction.FollowupAsync(error, ephemeral: true);
		}
	}

	private async Task HandleInteraction(SocketInteraction interaction)
	{
		var db = (CatCoreContext)_services.GetService(typeof(CatCoreContext));
		var context = new CatCoreInteractionContext(_client, interaction, db);
		await _interactionService.ExecuteCommandAsync(context, _services);
	}
}
