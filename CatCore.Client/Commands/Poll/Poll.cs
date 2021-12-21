using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatCore.Data;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Discord.Net;

namespace CatCore.ClientCommands
{
	public partial class PollCommands
	{
		[ComponentInteraction("poll.*.launch", true)]
		public async Task RespondWithPoll(string id)
		{
			Poll poll = await DBHelper.GetPollAsync(Convert.ToUInt64(id));
			List<PollRole> pollRoles = await DBHelper.GetPollRolesAsync(poll.Id);

			pollRoles = pollRoles.Count > 20
				? pollRoles.GetRange(0, 20)
				: pollRoles;

			int min = poll.Min > pollRoles.Count || poll.Min < 0 
				? pollRoles.Count 
				: poll.Min;
				
			int max = poll.Max > pollRoles.Count || poll.Max < 1
				? pollRoles.Count
				: poll.Max;

			ComponentBuilder cb = new();
			SelectMenuBuilder sb = new SelectMenuBuilder()
				.WithCustomId($"poll.{id}.result")
				.WithPlaceholder("select your roles")
				.WithMinValues(min)
				.WithMaxValues(max);

			List<SocketRole> roles = Context.Guild.Roles.ToList();

			pollRoles.OnEach(x => sb.AddOption(roles.Where(y => y.Id == x.RoleId).First().Name,
				x.RoleId.ToString(), x.Description,
				isDefault: (Context.User as IGuildUser).RoleIds.Any(z => z == x.RoleId)));


			pollRoles = pollRoles.Count > 20
				? pollRoles.GetRange(0, 20)
				: pollRoles;
			cb.WithSelectMenu(sb);

			await RespondAsync(embed: poll.GetEmbed().Build(), component: cb.Build(), ephemeral: true);
		}
	}
}
