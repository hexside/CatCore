namespace CatCore.Client.Commands;

public class CatCoreInteractionContext : SocketInteractionContext
{
	public CatCoreContext Db { get; }
	public User DbUser { get; }
	public Guild DbGuild { get; set; }

	public CatCoreInteractionContext(DiscordSocketClient client, SocketInteraction interaction, CatCoreContext db) 
		: base(client, interaction)
	{
		Db = db;
		DbUser = db.Users
			.Include(u => u.Pronouns)
			.Include(u => u.Messages)
				.ThenInclude(um => um.Message)
			.Include(u => u.MessageGroups)
				.ThenInclude(mg => mg.VisiableTo)
			.FirstOrDefault(u => u.DiscordID == User.Id);
			
		DbGuild = db.Guilds
			.Include(g => g.Polls)
				.ThenInclude(p => p.Roles)
			// .Include(g => g.RegexActions)
			// 	.ThenInclude(ra => ra.Conditions)
			.FirstOrDefault(g => g.DiscordId == Guild.Id);

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
		if (DbGuild is null)
		{
			DbGuild = new(Guild.Id);
			db.Guilds.Add(DbGuild);
			db.SaveChanges();
		}
	}
}
