using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utils;

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
			await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

			_client.InteractionCreated += _handleInteraction;
			// TODO: post interaction cleanup and error logging.

			_interactionService.SlashCommandExecuted += _slashCommandExecuted;
		}

		private async Task _slashCommandExecuted(SlashCommandInfo command, IInteractionContext context, IResult result)
		{
			if (result.IsSuccess)
			{
				await _logger.LogDebug($"sucesfully executed the command {command.Name}");
			}
			else
			{
				await _interactionFailHandle(command.Name, result, context.Interaction);
			}
		}

		private async Task _interactionFailHandle(string name, IResult result, IDiscordInteraction interaction)
		{
			string error = $"the interaction {name} failed with the error type {result.Error} : {result.ErrorReason}";
			await _logger.LogWarning(error);
			// try responding to the interaction
			try
			{
				await interaction.RespondAsync(error, ephemeral: true);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex);
				await interaction.FollowupAsync(error, ephemeral:true);
			}
		}

		private async Task _handleInteraction (SocketInteraction interaction)
		{
			var context = new SocketInteractionContext(_client, interaction);
			await _interactionService.ExecuteCommandAsync(context, _services);
		}
	}
}
