using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using CatCore.Data;

namespace CatCore.ClientCommands;

public partial class PollCommands
{
	[ComponentInteraction("poll.*.result", true)]
	public async Task AddPollRole(string id, string[] values)
	{
		await (Context.Interaction as SocketMessageComponent).DeferLoadingAsync(ephemeral: true);
		
		
		List<PollRole> roles = await DBHelper.GetPollRolesAsync(Convert.ToUInt64(id));
		SocketGuildUser user = Context.User as SocketGuildUser;
		List<ulong> selectedRoles = values.Select(x => Convert.ToUInt64(x)).ToList();

		List<ulong> addRoles = selectedRoles
			.Where(x => !user.Roles
				.Select(x => x.Id)
				.Contains(x))
			.ToList();

		List<ulong> removeRoles = roles
			.Where(x => !selectedRoles
				.Contains(x.RoleId))
			.Select(x => x.RoleId)
			.Where(x => user.Roles
				.Select(x => x.Id)
				.Contains(x))
			.ToList();
			
		// add roles to the user
		await user.AddRolesAsync(addRoles);

		// remove roles from the user
		await user.RemoveRolesAsync(removeRoles);

		string added = string.Join("\n", addRoles.Select(x => $"<@&{x}>"));
		string removed = string.Join("\n", removeRoles.Select(x => $"<@&{x}>"));
		
		if (added.Length > 1) added = "**Added the Roles**\n" + added;
		if (removed.Length > 1) removed = "**Removed the Roles**\n" + removed;
		await FollowupAsync(added.Length + removed.Length > 4 ? $"{added}\n{removed}" : "Your roles were not updated", allowedMentions:AllowedMentions.None);
	}
}
