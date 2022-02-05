using System.Reflection;
using CatCore.Client.TypeConverters;
using CatCore.Data;
using CatCore.Client;

namespace Client
{
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
				await _logger.LogVerbose("Adding TypeConverters").ConfigureAwait(false);
				_interactionService.AddGenericTypeConverter<Poll>(typeof(PollTypeConverter<>));
				_interactionService.AddGenericTypeConverter<Pronoun>(typeof(PronounTypeConverter<>));
				_interactionService.AddGenericTypeConverter<UserMessage>(typeof(UserMessageTypeConverter<>));
				await _interactionService.AddModulesAsync(Assembly.GetAssembly(typeof(CatCore.Client.Commands.PronounCommands)), _services);

				_client.InteractionCreated += HandleInteraction;
				
				// TODO: post interaction cleanup and error logging.

				_interactionService.SlashCommandExecuted += SlashCommandExecuted;
			}
			catch (Exception ex)
			{
				await _logger.LogCritical("Failed to start interactions service.", ex);
			}
		}

		private async Task SlashCommandExecuted(SlashCommandInfo command, IInteractionContext context, IResult result)
		{
			if (result.IsSuccess)
			{
				await _logger.LogDebug($"Successfully executed the command {command.Name}");
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
}
