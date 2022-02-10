using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace CatCore.Data
{
	public class Poll
	{
		public int PollId { get; set; }
		public int GuildId { get; set; }
		public Guild Guild { get; set; }
		public string? Title { get; set; }
		public string? Description { get; set; }
		public string? Footer { get; set; }
		public int Min { get; set; }
		public int Max { get; set; }
		public List<PollRole> Roles { get; set; } = new();
		public string? ImageUrl { get; set; }
		public int Color { get; set; }

		public EmbedBuilder GetEmbed()
			=> new EmbedBuilder()
				.WithTitle(Title)
				.WithDescription(Description)
				.WithFooter($"({PollId}) | {Footer}")
				.WithImageUrl(ImageUrl);

		public string ToString(bool longForm) => longForm
			? $"({PollId}) {Title} : {Description} : {Footer}"
			: $"({PollId}) {Title}";
	}
}
