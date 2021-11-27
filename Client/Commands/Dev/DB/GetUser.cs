using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
			private static readonly JsonSerializerOptions _jsonOptions = new()
			{
				WriteIndented = true
			};

			[SlashCommand("get-user", "gets the `IsDev` value from a specific user")]
			public async Task GetUser(IUser discordUser)
				=> await Context.Interaction.RespondAsync("```json\n" +
					JsonSerializer.Serialize(await DBHelper.GetUserAsync(discordUser.Id), _jsonOptions) + "\n```");
		}
	}
}