using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord;

namespace CatCore.Client.Commands
{
	public partial class Dev
	{
		public partial class Commands
		{
			[SlashCommand("update-global", "regester guild commands")]
			public async Task UpdateGlobal()
			{
				await Context.Interaction.DeferAsync(true);
				await Interactions.RegisterCommandsGloballyAsync();
				await Context.Interaction.FollowupAsync("updated global commands");
			}
		}
	}
}
