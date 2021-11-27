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
			[SlashCommand("rebuild-query-cache", "clear and recreateate cached sql queries")]
			public async Task RebuildQueryCache()
			{
				await Context.Interaction.DeferAsync(true);
				DBHelper.Init();
				await Context.Interaction.FollowupAsync("rebuild query cache");
			}
		}
	}
}