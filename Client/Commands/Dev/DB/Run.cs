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
			[SlashCommand("run", "Run raw sql code in the client")]
			public async Task RQ(string command, SqlCommandResponceType responceType)
			{
				await Context.Interaction.DeferAsync(true);
				string responce = responceType == SqlCommandResponceType.ConvertedReader
					? (await DBHelper.RunQueryAsync(command)).ReadAsText(2000)
					: (await DBHelper.RunNonQueryAsync(command)).ToString() + " Rows Effected,";
				await Context.Interaction.FollowupAsync(responce);
			}
		}
	}
}