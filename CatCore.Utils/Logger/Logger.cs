using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Webhook;
using System.Linq;

namespace CatCore.Utils;
public class Logger : IDisposable
{
	public List<LogMessage> Messages { get; private set; }
	public string Source { get; private set; }
	public string? WebhookUrl 
	{ 
		get => _webhookUrl;
		set
		{
			_webhookUrl = value;
			_client = new(value);
		}
	}
	public LogSeverity LogLevel { get; set; }
	public LogSeverity WebhookLevel { get; set; }
	public event Func<LogMessage, Task> LogFired;

	private string _webhookUrl { get; set; }
	private DiscordWebhookClient _client { get; set; }

	public Logger(string source, LogSeverity severity = LogSeverity.Info)
	{
		Source = source;
		Messages = new List<LogMessage>();
		LogLevel = severity;

		LogFired += _fireWebhook;
		Log("Logger", "Started Logger", LogSeverity.Info).ConfigureAwait(false);
	}

	public Logger(string source, string webhookUrl, LogSeverity severity = LogSeverity.Info, 
		LogSeverity webhookSeverity = LogSeverity.Warning)
	{
		Source = source;
		WebhookUrl = webhookUrl;
		LogLevel = severity;
		WebhookLevel = webhookSeverity;
		Messages = new List<LogMessage>();

		LogFired += _fireWebhook;
		Log("Logger", "Started Logger", LogSeverity.Info).ConfigureAwait(false);
	}

	private async Task _fireWebhook(LogMessage message)
	{
		if (_client is null)
			return;

		if (message.Severity > WebhookLevel)
			return;

		var builder = new EmbedBuilder()
			.WithTitle(message.Severity.ToString())
			.WithAuthor(message.Source)
			.WithDescription(message.Message)
			.WithCurrentTimestamp()
			.WithColor(message.Severity.GetColor());
		if (message.Exception != null)
			builder.WithFooter(message.Exception?.ToString() ?? "[invalid exception]",
				"https://cdn.discordapp.com/emojis/842172192401915971.png?size=64");
		
		await _client.SendMessageAsync(embeds: new [] { builder.Build() });
	}

	public async Task Log (LogMessage message)
	{
		Messages.Add(message);
		await LogFired.Invoke(message);
	}

	public async Task Log (string source, string message, LogSeverity severity, Exception? exception = null)
	{
		LogMessage lMessage = new (severity, source, message, exception);
		await Log(lMessage);
	}

	#region log methods
	public async Task LogDebug (string message, Exception? exception = null) 
		=> await LogDebug(Source, message, exception);
	public async Task LogDebug (string source, string message, Exception? exception = null)
		=> await Log(source, message, LogSeverity.Debug, exception);

	public async Task LogVerbose (string message, Exception? exception = null) 
		=> await LogVerbose(Source, message, exception);
	public async Task LogVerbose (string source, string message, Exception? exception = null)
		=> await Log(source, message, LogSeverity.Verbose, exception);

	public async Task LogInfo (string message, Exception? exception = null) 
		=> await LogInfo(Source, message, exception);
	public async Task LogInfo (string source, string message, Exception? exception = null)
		=> await Log(source, message, LogSeverity.Info, exception);

	public async Task LogWarning (string message, Exception? exception = null) 
		=> await LogWarning(Source, message, exception);
	public async Task LogWarning (string source, string message, Exception? exception = null)
		=> await Log(source, message, LogSeverity.Warning, exception);

	public async Task LogError (string message, Exception? exception = null) 
		=> await LogError(Source, message, exception);
	public async Task LogError (string source, string message, Exception? exception = null)
		=> await Log(source, message, LogSeverity.Error, exception);

	public async Task LogCritical (string message, Exception? exception = null) 
		=> await LogCritical(Source, message, exception);
	public async Task LogCritical (string source, string message, Exception? exception = null)
		=> await Log(source, message, LogSeverity.Critical, exception);

	public async Task LogTest() => 
		await Task.Run(() => Enum.GetValues<LogSeverity>().OnEach(async e => 
			await Log("Logger", $"This is a {e} message.", e)));
	#endregion

	public void Dispose()
	{
		Messages.Clear();
		_client.Dispose();
		LogFired = null;
		GC.SuppressFinalize(this);
	}
}
