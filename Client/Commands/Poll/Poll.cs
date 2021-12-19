using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBManager;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Discord.Net;

namespace Client.Commands
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

			ComponentBuilder cb = new();
			SelectMenuBuilder sb = new SelectMenuBuilder()
				.WithCustomId($"poll.{id}.result")
				.WithPlaceholder("select your roles")
				.WithMinValues(0);

			List<SocketRole> roles = Context.Guild.Roles.ToList();

			pollRoles.OnEach(x => sb.AddOption(roles.Where(y => y.Id == x.RoleId).First().Name,
				x.RoleId.ToString(), x.Description,
				isDefault: (Context.User as IGuildUser).RoleIds.Any(z => z == x.RoleId)));


			pollRoles = pollRoles.Count > 20
				? pollRoles.GetRange(0, 20)
				: pollRoles;
			cb.WithSelectMenu(sb.WithMaxValues(sb.Options.Count));

			await RespondAsync(embed: poll.GetEmbed().Build(), component: cb.Build(), ephemeral:true);
		}
	}
}
