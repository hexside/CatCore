global using Discord;
global using Discord.Interactions;
global using Discord.WebSocket;
global using CatCore.Utils;
global using CatCore.Data;
global using Microsoft.EntityFrameworkCore;
global using CatCore.Client.Commands;

using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Text.Json;

namespace CatCore.Client;

internal class Program
{
	private static Logger _logger;

	private static ServiceProvider _configureServices() => new ServiceCollection()
		.AddSingleton(new ClientSettings("clientSettings.json"))
		.AddDbContext<CatCoreContext>()
		.AddSingleton(new DiscordSocketClient(new()
		{
			LogLevel = LogSeverity.Debug,
			GatewayIntents = GatewayIntents.Guilds
		}))
		.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>(),
			new()
			{
				LogLevel = LogSeverity.Debug,
				DefaultRunMode = RunMode.Async
			}))
		.AddSingleton<CommandHandler>(x => new(x.GetRequiredService<DiscordSocketClient>(),
			x.GetRequiredService<InteractionService>(), x, x.GetRequiredService<ClientSettings>()))
		.AddSingleton(JsonSerializer.Deserialize<List<ToneTag>>(File.ReadAllText("tags.json")))
		.BuildServiceProvider();

	private static async Task Main(string[] _)
	{
		using var services = _configureServices();
		var client = services.GetRequiredService<DiscordSocketClient>();
		var commands = services.GetRequiredService<InteractionService>();
		var handler = services.GetRequiredService<CommandHandler>();
		var settings = services.GetRequiredService<ClientSettings>();
		client.Log += x =>
		{
			_logger.Log(x);
			return Task.CompletedTask;
		};
		commands.Log += x =>
		{
			_logger.Log(x);
			return Task.CompletedTask;
		};
		handler.Log += x =>
		{
			_logger.Log(x);
			return Task.CompletedTask;
		};

		_logger = new("Client", settings.WebhookUrl, LogSeverity.Debug);
		_logger.LogFired += x => Task.Run(() => Console.WriteLine(x.ToFormattedString()));
		_logger.LogInfo("CatCore " + Assembly.GetEntryAssembly().GetName().Version);

		await handler.InitializeAsync();

		int count = commands.SlashCommands.Count + commands.ComponentCommands.Count + commands.ContextCommands.Count;
		_logger.LogInfo($"Loaded {count} commands.");
		await client.SetGameAsync($"{count} commands.", type: ActivityType.Listening);

		await client.LoginAsync(TokenType.Bot, settings.Token);
		await client.StartAsync();

		await Task.Delay(-1);
	}
}
