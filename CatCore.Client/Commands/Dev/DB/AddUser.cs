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

			[SlashCommand("add-user", "gets the `IsDev` value from a specific user")]
			public async Task AddUser(IUser discordUser, bool isDev = false)
			{
				await DBHelper.AddUserAsync(discordUser.Id, isDev);
				await Context.Interaction.RespondAsync("```json\n" +
					JsonSerializer.Serialize(await DBHelper.GetUserAsync(discordUser.Id), _jsonOptions) + "\n```");
			}
		}
	}
}