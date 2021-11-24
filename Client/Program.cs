using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CottageDwellingAdditions;

namespace Client;

internal class Program
{
	private static bool _firstReady = true;
	private static Logger _logger;
	private static ClientSettings _settings = new("clientSettings.json");

	static ServiceProvider _configureServices() => new ServiceCollection()
		.AddSingleton(new DiscordSocketClient(new() { LogLevel = LogSeverity.Debug }))
		.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>(), 
			new() { LogLevel=LogSeverity.Debug}))
		.AddSingleton<CommandHandler>(x => new(x.GetRequiredService<DiscordSocketClient>(),
			x.GetRequiredService<InteractionService>(), x))
		.BuildServiceProvider();

	private static async Task Main(string[] _)
	{
		using var services = _configureServices();
		var client = services.GetRequiredService<DiscordSocketClient>();
		var commands = services.GetRequiredService<InteractionService>();
		var handler = services.GetRequiredService<CommandHandler>();

		client.Log += async (x) => await _logger.Log(x);
		commands.Log += async (x) => await _logger.Log(x);

		_logger = new("Client", _settings.WebhookUrl, LogSeverity.Debug);
		_logger.LogFired += x => Task.Run(() => Console.WriteLine(x.ToFormattedString()));
		await _logger.LogInfo("CatCore " + Assembly.GetEntryAssembly().GetName().Version);

		handler.Log += async x => await _logger.Log(x);
		await handler.InitializeAsync();

		client.Ready += async () =>
		{
			if (_firstReady)
			{
				_firstReady = false;
				await _logger.LogVerbose("starting interaction service").ConfigureAwait(false);

				if (_settings.DebugMode) await commands.RegisterCommandsToGuildAsync(_settings.DebugGuildId, true);
				else await commands.RegisterCommandsGloballyAsync(true);
			}
		};

		await client.LoginAsync(TokenType.Bot, _settings.Token);
		await client.StartAsync();

		await Task.Delay(-1);
	}
}