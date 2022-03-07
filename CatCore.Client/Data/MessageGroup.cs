namespace CatCore.Data;

public class MessageGroup
{
	public List<Message> Messages { get; set; } = new();
	public int MessageGroupId { get; set; }

	public string Name { get; set; }
	public bool IsPublic { get; set; } = true;
	public List<User>? VisiableTo { get; set; } = new();

	public async Task MessageAllUsersAsync(Message message, CatCoreDbContext db)
	{
		Messages.Add(message);
		db.MessageGroups.Update(this);
		(await db.Users
			.Where(x => x.MessageGroups.Contains(this))
			.ToListAsync())
			.OnEach(x =>
			{
				x.Messages.Add(new(message, x));
				db.Users.Update(x);
			});
		await db.SaveChangesAsync();
	}
}
