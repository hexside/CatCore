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
			public InteractionService Interactions { get; set; }
			[SlashCommand("update-guild", "regester guild commands")]
			public async Task UpdateGuild(
			[Summary("guild", "the ID of the guild to make commands in, null for this guild.")] string? guildIdStr = null)
			{
				await Context.Interaction.DeferAsync(true);
				ulong guildId = guildIdStr is not null 
					? Convert.ToUInt64(guildIdStr) 
					: Context.Guild.Id;
				await Interactions.RegisterCommandsToGuildAsync(guildId);
				await Context.Interaction.FollowupAsync("updated guild commands in " + guildId);
			}
		}
	}
}
