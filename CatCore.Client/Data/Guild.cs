namespace CatCore.Data;

public class Guild
{
	public int GuildId { get; set; }
	public List<Poll> Polls { get; set; } = new();
	public List<RegexAction> RegexActions { get; set; } = new();
	public ulong DiscordId { get; set; }
	public ulong MessageFlagChannelId { get; set; }

	public Guild() {}
	
	public Guild(ulong discordId)
	{
		DiscordId = discordId;
	}
}
