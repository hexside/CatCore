using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace CatCore.Client.Commands;
public partial class UnGrouped : InteractionModuleBase<SocketInteractionContext>
{
	private static readonly List<AutocompleteResult> _activities = new()
	{
		new("Youtube Together", "880218394199220334"),
		new("Youtube Together Old", "755600276941176913"),
		new("Poker Night", "755827207812677713"),
		new("Betrayal.io", "773336526917861400"),
		new("Fishington.io", "814288819477020702"),
		new("Chess in the Park", "832012774040141894"),
		new("Sketchy Artist", "879864070101172255"),
		new("Awkword", "879863881349087252"),
		new("Putts", "832012854282158180"),
		new("Doodle Crew", "878067389634314250"),
		new("Letter Tile", "879863686565621790"),
		new("Word Snacks", "879863976006127627"),
		new("Spell Cast", "852509694341283871"),
		new("CG3 Prod", "832013003968348200"),
		new("CG4 Prod", "832025144389533716"),
	};


	[SlashCommand("activity", "autcomplete stuff")]
	public async Task Activity(
		[Summary("activity", "the name or id of the activity to launch")] 
		[Autocomplete(typeof(ActivityAutocompleteProvider))]
		string activity,
		[Summary("channel", "the channel to start the activity in")]
		SocketVoiceChannel channel)
	{
		ulong id = 0;
		try
		{
			id = Convert.ToUInt64(activity);
		}
		catch
		{
			id = Convert.ToUInt64((string)_activities.Where(x => x.Name == activity)
				.Select(x => x.Value).FirstOrDefault("-1"));
		}
		string invite = "discord://discord.gg/" + (await channel.CreateInviteToApplicationAsync(id, 21600)).Code;
		ComponentBuilder cb = new ComponentBuilder()
			.WithButton("Join the activity", style: ButtonStyle.Link, url: invite);
		await Context.Interaction.RespondAsync("click the button below to launch the activity", components: cb.Build());
	}

	public class ActivityAutocompleteProvider : AutocompleteHandler
	{
		public override Task<AutocompletionResult> GenerateSuggestionsAsync(IInteractionContext context, 
			IAutocompleteInteraction autocompleteInteraction, IParameterInfo parameter, IServiceProvider services)
		{
			string content = autocompleteInteraction.Data.Current.Value.ToString().ToLower();
			List<AutocompleteResult> results = _activities.Where(x =>
				x.Name.Contains(content) || x.Value.ToString().Contains(content)).OrderByDescending(x => 
				Regex.Matches(x.Name, Regex.Escape(content)).Count + Regex.Matches((string)x.Value, Regex.Escape(content)).Count)
				.ToList();
			return Task.FromResult(AutocompletionResult.FromSuccess(results));
		}

		protected override string GetLogString(IInteractionContext context) => nameof(ActivityAutocompleteProvider);
	}
}