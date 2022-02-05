namespace CatCore.Client;

public class CatCoreInteractionContext : SocketInteractionContext
{
	public CatCoreContext Db { get; }
	public User DbUser { get; }
	
	public CatCoreInteractionContext(DiscordSocketClient client, SocketInteraction interaction, CatCoreContext db) 
		: base(client, interaction)
	{
		Db = db;
		DbUser = db.Users
			.Include(x => x.Pronouns)
			.Include(x => x.Messages)
				.ThenInclude(x => x.Message)
			.FirstOrDefault(x => x.DiscordID == User.Id);
		if (DbUser is null)
		{
			DbUser = new(false, User.Id);
			db.Users.Add(DbUser);
			db.SaveChanges();
		}
	}
}
