using System.Text.RegularExpressions;

namespace CatCore.Data;

public class RegexAction
{
	public int RegexActionId { get; set; }

	public bool Valid { get; set; }
	public string? ActionName { get; set; }
	public int GuildId { get; set; }
	public Guild Guild { get; set; }
	public string? RegexString { get; set; }
	public bool CleanMessage { get; set; }
	public bool RemoveWhitespace { get; set; }
	public RegexActionType Type { get; set; }

	public async Task ExecuteAsync(IMessage message)
	{
		if (message.Author is not IGuildUser user) return;
		
		string content = CleanMessage
			? message.CleanContent
			: message.Content;

		if (!ShouldExecute(content)) return;

		switch (Type)
		{
			case RegexActionType.MessageFlag:
				{
					var channel = await user.Guild.GetTextChannelAsync(Guild.MessageFlagChannelId);
					string warning = $"**FLAGGED MESSAGE** A message by {user.Mention} ({user.Id}) " +
						$"Matched the filter {ActionName} (`{RegexString}`)";
					var eb = new EmbedBuilder()
						.WithAuthor(message.Author)
						.WithColor(Color.Red)
						.WithUrl(message.GetJumpUrl())
						.WithTitle("Flagged Message")
						.WithDescription(message.Content);
					await channel.SendMessageAsync(warning, embed:eb.Build());
				}
				break;
			case RegexActionType.MessageDelete:
				{
					await message.DeleteAsync();
					var channel = await user.Guild.GetTextChannelAsync(Guild.MessageFlagChannelId);
					string warning = $"**DELETED MESSAGE** A message by {user.Mention} ({user.Id}) " +
						$"Matched the filter {ActionName} (`{RegexString}`)";
					var eb = new EmbedBuilder()
						.WithAuthor(message.Author)
						.WithColor(Color.Red)
						.WithTitle("Deleted Message")
						.WithDescription(message.Content);
					await channel.SendMessageAsync(warning, embed:eb.Build());
				}
				break;
		}
	}
	
	public bool ShouldExecute (string content)
	{
		if (CleanMessage) content = Format.StripMarkDown(content).Normalize();
		if (RemoveWhitespace) content = Regex.Replace(content, @"\s+", "");
		if (RegexString is null) return false;

		return Regex.IsMatch(content, RegexString, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(10));
	}
}
