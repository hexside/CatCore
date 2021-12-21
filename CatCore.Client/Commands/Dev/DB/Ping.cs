using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
			[SlashCommand("ping", "update commands regestered in the guild")]
			public async Task Ping()
				=> await Context.Interaction.RespondAsync($"the server {(DBHelper.Ping()?"is":"is not")} responding",
					ephemeral:true);
		}
	}
}