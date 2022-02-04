namespace CatCore.Data
{
	public class User
	{
		public bool IsDev { get; set; }
		public int UserID { get; set; }
		public ulong DiscordID { get; set; }
		
		public List<Pronoun> Pronouns { get; set; }

		public User(bool isDev, ulong id)
		{
			DiscordID = id;
			IsDev = isDev;
		}

		public User() { }
	}
}
