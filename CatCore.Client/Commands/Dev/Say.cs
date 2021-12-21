using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord;

namespace CatCore.Client.Commands;
public partial class Dev
{
	[SlashCommand("say", "echo a message")]
	public async Task Say(
		[Summary("message", "the message to send")]
		string message, 
		[Summary("as-file", "send the message as a file instead of text")]
		bool messageAsFile = false,
		[Summary("ephemeral", "should the message be ephemeral")]
		bool ephemeral = false)
	{
		await Context.Interaction.DeferAsync(ephemeral);
		if (messageAsFile)
		{
			using MemoryStream ms = new();
			using StreamWriter sw = new(ms);
			sw.Write(message);
			sw.Flush();
			ms.Position = 0;

			await Context.Interaction.FollowupWithFileAsync(ms, "file.txt");
		}
		else
		{
			await Context.Interaction.FollowupAsync(message);
		}
	}
}
