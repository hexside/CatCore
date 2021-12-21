using System.Threading.Tasks;
using CatCore.Client.Autocomplete;
using Discord.Interactions;
using CatCore.Data;
using System;

namespace CatCore.Client.Commands;
public partial class PollCommands
{
	[SlashCommand("update", "modify a already created poll")]
	public async Task UpdatePoll(
		[Autocomplete(typeof(PollAutocompleteProvider))]
		[Summary("poll", "the poll to update.")]
		Poll oldPoll,
		[Summary(null, "The name of the poll.")]
		string name = null,
		[Summary(null, "polls description.")]
		string? description = null,
		[Summary(null, "polls embed footer.")]
		string? footer = null,
		[Summary(null, "The smallest number of options a user can choose (defaults to 1).")]
		int min = -1,
		[Summary(null, "The largest number of options a user can choose (defaults to total options if too large.)")]
		int max = -1
	)
	{
		Poll newPoll = new()
		{
			Description = description ?? oldPoll.Description,
			Footer = footer ?? oldPoll.Footer,
			Id = oldPoll.Id,
			Max = max < 0 ? oldPoll.Max : max,
			Min = min < 0 ? oldPoll.Min : min,
			Title = name ?? oldPoll.Title
		};
		await DBHelper.UpdatePollAsync(oldPoll, newPoll);
		await RespondAsync("Updated the poll", new[] { newPoll.GetEmbed().Build() }, ephemeral: true);
	}
}