namespace CatCore.Client.Commands;

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
			.Include(x => x.MessageGroups)
				.ThenInclude(x => x.VisiableTo)
			.FirstOrDefault(x => x.DiscordID == User.Id);
		if (DbUser is null)
		{
			var group = db.MessageGroups.FirstOrDefault(x => x.Name == "@everyone");
			if (group is null)
			{
				group = new()
				{
					IsPublic = false,
					Name = "@everyone"
				};
				db.MessageGroups.Add(group);
			}
			DbUser = new(true, User.Id);
			DbUser.MessageGroups.Add(group);
			db.Users.Add(DbUser);
			db.SaveChanges();
		}
	}
}
