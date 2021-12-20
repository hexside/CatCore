using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using Utils;
using DBManager;
using System.Text.Json;

namespace Client;

internal class Program
{
	private static bool _firstReady = true;
	private static Logger _logger;
	private static readonly ClientSettings _settings = new("clientSettings.json");

	private static ServiceProvider _configureServices() => new ServiceCollection()
		.AddSingleton(new DiscordSocketClient(new() 
		{ 
			LogLevel = LogSeverity.Debug, 
			GatewayIntents = GatewayIntents.Guilds 
		}))
		.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>(), 
			new() 
			{ 
				LogLevel=LogSeverity.Debug, 
				DefaultRunMode = RunMode.Async
		}))
		.AddSingleton<CommandHandler>(x => new(x.GetRequiredService<DiscordSocketClient>(),
			x.GetRequiredService<InteractionService>(), x))
		.AddSingleton(new DBHelper(_settings.DBConnectionString))
		.AddSingleton(JsonSerializer.Deserialize<List<ToneTag>>(File.ReadAllText("tags.json")))
		.BuildServiceProvider();

	private static async Task Main(string[] _)
	{
		using var services = _configureServices();
		var client = services.GetRequiredService<DiscordSocketClient>();
		var commands = services.GetRequiredService<InteractionService>();
		var handler = services.GetRequiredService<CommandHandler>();
		var db = services.GetRequiredService<DBHelper>();

		client.Log += async (x) => await _logger.Log(x);
		commands.Log += async (x) => await _logger.Log(x);
		db.Log += async (x) => await _logger.Log(x);
		handler.Log += async x => await _logger.Log(x);
		

		_logger = new("Client", _settings.WebhookUrl, LogSeverity.Debug);
		_logger.LogFired += x => Task.Run(() => Console.WriteLine(x.ToFormattedString()));
		await _logger.LogInfo("CatCore " + Assembly.GetEntryAssembly().GetName().Version);

		await handler.InitializeAsync();
		db.Init();

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