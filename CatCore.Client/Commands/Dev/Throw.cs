using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord;
using Discord.WebSocket;

namespace CatCore.Client.Commands
{
	public partial class Dev
	{
		[SlashCommand("throw", "throw an unhandled exception")]
		public async Task Throw(
		[Summary("defer", "how should the interaction be defered")] DeferType defer,
		[Summary("reason", "what should the exception message be")] string reason = "idk, you tell me")
		{
			if (defer == DeferType.DeferEphemeraly) await Context.Interaction.DeferAsync(true);
			if (defer == DeferType.Defer) await Context.Interaction.DeferAsync(false);
			throw new Exception(reason);
		}

		public enum DeferType
		{
			DontDefer,
			Defer,
			DeferEphemeraly
		}
	}
}
