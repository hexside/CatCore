namespace CatCore.Data;

public class Message
{
	public int MessageId { get; set; }
	public int? MessageGroupId { get; set; }
	public MessageGroup? MessageGroup { get; set; }

	public string Title { get; set; }
	public string Description { get; set; }
	public string Footer { get; set; }
	public string ImageUrl { get; set; }
	public int Color { get; set; }

	/// <summary>
	/// 	Internal not set by admins sending the message, not visible to users.
	/// </summary>
	public string AdminMessage { get; set; }
	
	/// <summary>
	/// 	Adds the message to a collection of users.
	/// </summary>
	/// <param name="users">The users to add the message to.</param>
	/// <param name="db">The db context to work in.</param>
	public async Task AddToAsync(IEnumerable<User> users, CatCoreContext db)
	{
		if (MessageId == default) await db.Messages.AddAsync(this);
		foreach (User user in users)
		{
			user.Messages.Add(new(this, user));
			db.Users.Update(user);
		}
	}

	public Embed GetEmbed()
		=> new EmbedBuilder()
			.WithTitle(Title)
			.WithDescription(Description)
			.WithFooter($"{MessageId} | {Footer}")
			.WithImageUrl(ImageUrl)
			.WithColor(new(0x2E8B57))
			.Build();
}
