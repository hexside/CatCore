using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace DBManager
{
	public class Poll
	{
		[SqlReadonly]
		[SqlColumn("id")]
		public ulong Id { get; set; }

		[SqlColumn("guildId")]
		public ulong GuildId { get; set; }

		[SqlColumn("title")]
		public string? Title { get; set; }

		[SqlColumn("description")]
		public string? Description { get; set; }

		[SqlColumn("footer")]
		public string? Footer { get; set; }

		public EmbedBuilder GetEmbed()
			=> new EmbedBuilder()
				.WithTitle(Title)
				.WithDescription(Description)
				.WithFooter($"({Id}) | {Footer}");

		public string ToString(bool longForm) => longForm
			? $"({Id}) {Title} : {Description} : {Footer}"
			: $"({Id}) {Title}";
	}
}
