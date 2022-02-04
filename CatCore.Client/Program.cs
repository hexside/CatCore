global using Discord;
global using Discord.Net;
global using Discord.Interactions;
global using Discord.WebSocket;
global using CatCore.Utils;
global using CatCore.Data;
global using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Text.Json;

namespace Client;

internal class Program
{
	private static bool _firstReady = true;
	private static Logger _logger;
	private static readonly ClientSettings _settings = new("clientSettings.json");

	private static ServiceProvider _configureServices() => new ServiceCollection()
		.AddDbContext<CatCoreContext>(ServiceLifetime.Transient)
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
		.AddSingleton(JsonSerializer.Deserialize<List<ToneTag>>(File.ReadAllText("tags.json")))
		.BuildServiceProvider();

	private static async Task Main(string[] _)
	{
		using var services = _configureServices();
		var client = services.GetRequiredService<DiscordSocketClient>();
		var commands = services.GetRequiredService<InteractionService>();
		var handler = services.GetRequiredService<CommandHandler>();

		client.Log += async (x) => await _logger.Log(x);
		commands.Log += async (x) => await _logger.Log(x);
		handler.Log += async x => await _logger.Log(x);
		

		_logger = new("Client", _settings.WebhookUrl, LogSeverity.Debug);
		_logger.LogFired += x => Task.Run(() => Console.WriteLine(x.ToFormattedString()));
		await _logger.LogInfo("CatCore " + Assembly.GetEntryAssembly().GetName().Version);

		await handler.InitializeAsync();

		client.Ready += async () =>
		{
			if (_firstReady)
			{
				_firstReady = false;
				await _logger.LogVerbose("Starting interaction service.").ConfigureAwait(false);
				try
				{
					if (_settings.DebugMode) await commands.RegisterCommandsToGuildAsync(_settings.DebugGuildId, true);
					else await commands.RegisterCommandsGloballyAsync(true);
					
					int count = commands.SlashCommands.Count + commands.ComponentCommands.Count + commands.ContextCommands.Count;
					await _logger.LogInfo($"Loaded {count} commands.");
					await client.SetGameAsync($"{count} commands.", type: ActivityType.Listening);
				}
				catch (HttpException ex)
				{
					await _logger.LogCritical($"Failed to regester commands because {ex}, (see {JsonSerializer.Serialize(ex.Errors)})").ConfigureAwait(false);
				}
			}
		};

		await client.LoginAsync(TokenType.Bot, _settings.Token);
		await client.StartAsync();

		await Task.Delay(-1);
	}
}
