using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using DBManager;

namespace Client.Commands
{
	public partial class PollCommands
	{
		[SlashCommand("new", "add a new poll")]
		public async Task New(
			[Summary(null, "The name of the poll.")]
			string name,
			[Summary(null, "polls description.")]
			string? description = null,
			[Summary(null, "polls embed footer.")]
			string? footer = null,
			[Summary(null, "The smallest number of options a user can choose (defaults to total options if too small).")]
			int? min = 0,
			[Summary(null, "The largest number of options a user can choose (defaults to total options if too large.)")]
			int? max = 0)
		{
			Poll poll = new()
			{
				Title = name,
				Description = description ?? "",
				Footer = footer ?? "",
				GuildId = Context.Guild.Id,
				Max = max.Value,
				Min = min.Value
			};

			await DBHelper.AddPollAsync(poll);
			Poll dbPoll = (await DBHelper.GetGuildPollsAsync(Context.Guild.Id))
				.Where(x => x.Footer == poll.Footer)
				.Where(x => x.Title == poll.Title)
				.Where(x => x.Description == poll.Description)
				.FirstOrDefault();

			await RespondAsync("Added the poll!", embed: dbPoll.GetEmbed().Build(), ephemeral: true);
		}
	}
}
