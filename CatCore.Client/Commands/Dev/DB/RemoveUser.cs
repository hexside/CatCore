using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord;
using CatCore.Data;

namespace CatCore.ClientCommands
{
	public partial class Dev
	{
		public partial class DB
		{

			[SlashCommand("remove-user", "remove a user from the database")]
			public async Task RemoveUSer(IUser discordUser)
			{
				await DBHelper.RemoveUserAsync(await DBHelper.GetUserAsync(discordUser.Id));
				await RespondAsync("removed the user");
			}
		}
	}
}