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
		public async Task New(string name, string? description = null, string? footer = null)
		{
			Poll poll = new()
			{
				Title = name,
				Description = description ?? "",
				Footer = footer ?? "",
				GuildId = Context.Guild.Id
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
