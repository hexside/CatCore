namespace CatCore.Data
{
	public class User
	{
		public bool IsDev { get; set; }
		public int UserID { get; set; }
		public ulong DiscordID { get; set; }
		public List<UserMessage> Messages { get; set; } = new();
		public List<MessageGroup> MessageGroups { get; set; } = new();
		public bool AnyPronouns { get; set; } = false;

		public List<Pronoun> Pronouns { get; set; } = new();

		public User(bool isDev, ulong id)
		{
			DiscordID = id;
			IsDev = isDev;
		}

		public User() { }

		public List<Embed> GetEmbeds()
			=> Messages
				.Where(x => !x.IsRead)
				.Select(x => x.Message.GetEmbed())
				.ToList();
	}
}
