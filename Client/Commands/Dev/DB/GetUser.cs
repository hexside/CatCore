using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord;
using DBManager;

namespace Client.Commands
{
	public partial class Dev
	{
		public partial class DB
		{
			[SlashCommand("get-user", "gets the `IsDev` value from a specific user")]
			public async Task GetUser(IUser discordUser)
			{
				Query<User> user = new(DBHelper, $"select * from users where (discordId={discordUser.Id})");
				await Context.Interaction.RespondAsync(user.Run(new()).First().IsDev.ToString());
			}
		}
	}
}