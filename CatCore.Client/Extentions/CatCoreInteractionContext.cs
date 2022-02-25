namespace CatCore.Client.Commands;

public class CatCoreInteractionContext : SocketInteractionContext
{
	public CatCoreContext Db { get; }
	public User DbUser { get; }
	public Guild DbGuild { get; set; }

	public CatCoreInteractionContext(DiscordSocketClient client, SocketInteraction interaction, IServiceProvider services)
		: base(client, interaction)
	{
		Db = (CatCoreContext)services.GetService(typeof(CatCoreContext));
		var settings = (ClientSettings)services.GetService(typeof(ClientSettings));
		DbUser = Db.Users
			.Include(u => u.Pronouns)
			.Include(u => u.Messages)
				.ThenInclude(um => um.Message)
			.Include(u => u.MessageGroups)
				.ThenInclude(mg => mg.VisiableTo)
			.FirstOrDefault(u => u.DiscordID == User.Id);

		DbGuild = Db.Guilds
			.Include(g => g.Polls)
				.ThenInclude(p => p.Roles)
			.Include(g => g.RegexActions)
			.FirstOrDefault(g => g.DiscordId == Guild.Id);

		if (DbUser is null)
		{
			var group = Db.MessageGroups.FirstOrDefault(x => x.Name == "@everyone");
			if (group is null)
			{
				group = new()
				{
					IsPublic = false,
					Name = "@everyone"
				};
				Db.MessageGroups.Add(group);
			}
			DbUser = new(settings.Devs.Contains(User.Id), User.Id);
			DbUser.MessageGroups.Add(group);
			Db.Users.Add(DbUser);
			Db.SaveChanges();
		}
		if (DbGuild is null)
		{
			DbGuild = new(Guild.Id);
			Db.Guilds.Add(DbGuild);
			Db.SaveChanges();
		}
	}
}
