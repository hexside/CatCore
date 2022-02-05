namespace CatCore.Data;

public class UserMessage
{
	public int UserMessageId { get; set; }
	public Message Message { get; set; }
	public int MessageId { get; set; }
	public User User { get; set; }
	public int UserId { get; set; }
	
	public bool IsRead { get; set; }
	public bool IsSuppressed { get; set; }
	public bool IsNotifiable => !IsRead && !IsSuppressed;

	public UserMessage(Message message, User user)
	{
		Message = message;
		User = user;
	}

	public UserMessage() {}
}
