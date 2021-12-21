using System.Threading.Tasks;
using Discord;
using DBManager;
using Discord.Interactions;

namespace Client.Commands;

public partial class Dev
{
	public partial class DB
	{
		[SlashCommand("update-user", "updates a user in the database.")]
		public async Task UpdateUser(IUser discordUser, bool isDev)
		{
			User user = await DBHelper.GetUserAsync(discordUser.Id);
			User newUser = new()
			{
				InternalId = user.InternalId,
				IsDev = isDev
			};
			
			await new DBWriter<User>(DBHelper, "users", user, WriteAction.Update, newUser).RunAsync();
			await RespondAsync("Updated the user", ephemeral:true);
		}
	}
}