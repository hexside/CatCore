namespace CatCore.Data;

public class Message
{
	public int MessageId { get; set; }
	public int UserId { get; set; }
	public User User { get; set; }
	
	public string Title { get; set; }
	public string Description { get; set; }
	public string Footer { get; set; }
	public string ImageUrl { get; set; }
	
	/// <summary>
	/// 	Internal not set by admins sending the message, not visible to users.
	/// </summary>
	public string AdminMessage { get; set; }
	
	/// <summary>
	/// 	Adds the message to a collection of users.
	/// </summary>
	/// <param name="users">The users to add the message to.</param>
	/// <param name="db">The db context to work in.</param>
	public async Task AddTo(IEnumerable<User> users, CatCoreContext db)
	{
		if (MessageId == default) await db.Messages.AddAsync(this);
		foreach (User user in users)
		{
			user.Messages.Add(new(this, user));
			db.Users.Update(user);
		}
		await db.SaveChangesAsync();
	}
}
