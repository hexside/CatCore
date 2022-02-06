using CatCore.Client.Autocomplete;

namespace CatCore.Client.Commands;

[Group("mail", "mail")]
public class MailCommands : InteractionModuleBase<CatCoreInteractionContext> 
{
	[SlashCommand("inbox", "Search your mail for a message from the devs.")]
	public async Task Mail
	(
		[Summary("filter", "What mail do you want to see?")] MailSearchType searchType,
		[Autocomplete(typeof(UserMessageAutocompleteProvider))]
		[Summary(null, "Search for a message.")] UserMessage message
	)
	{
		message.IsRead = true;
		Context.Db.UserMessages.Update(message);
		await Context.Db.SaveChangesAsync();

		await RespondAsync(embed: message.Message.GetEmbed(), ephemeral: true);
	}
	
	public enum MailSearchType
	{
		Unread,
		Read,
		All
	}
	
	[SlashCommand("join", "Join a message group to get notifications from the developers.")]
	public async Task JoinGroup
	(
		[Autocomplete(typeof(MessageGroupAutocompleteProvider))]
		[Summary(null, "What group would you like to join?")] MessageGroup group
	)
	{
		if(!group.IsPublic && !group.VisiableTo.Contains(Context.DbUser))
		{
			await RespondAsync("You don't have permission to manage subscriptions in this group.", ephemeral: true);
			return;
		}
		if (Context.DbUser.MessageGroups.Contains(group))
		{
			await RespondAsync("You are already in this group.", ephemeral: true);
		}
		Context.DbUser.MessageGroups.Add(group);
		Context.Db.Users.Update(Context.DbUser);
		await Context.Db.SaveChangesAsync();
	}
	
	[SlashCommand("leave", "Exit a message group to stop geting notifications from the developers.")]
	public async Task ExitGroup
	(
		[AutocompleteFromUserAtribute]
		[Autocomplete(typeof(MessageGroupAutocompleteProvider))]
		[Summary(null, "What group would you like to exit?")] MessageGroup group
	)
	{
		if(!group.IsPublic && !group.VisiableTo.Contains(Context.DbUser))
		{
			await RespondAsync("You don't have permission to manage subscriptions in this group.", ephemeral: true);
			return;
		}
		if(!Context.DbUser.MessageGroups.Contains(group))
		{
			await RespondAsync("You are not in this group.", ephemeral: true);
		}
		Context.DbUser.MessageGroups.Remove(group);
		Context.Db.Users.Update(Context.DbUser);
		await Context.Db.SaveChangesAsync();
	}

	[ComponentInteraction("user.notifications.dismiss", true)]
	public async Task DismissNotifications()
	{
		Context.DbUser.Messages
			.Where(x => x.IsNotifiable)
			.OnEach(x => x.IsSuppressed = true);

		Context.Db.Update(Context.DbUser);
		await Context.Db.SaveChangesAsync();

		await RespondAsync("Hid your unread messages, remember you can always read them by running **`/mail inbox`**",
			ephemeral: true);
	}
}
