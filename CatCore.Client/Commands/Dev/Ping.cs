﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord;

namespace CatCore.ClientCommands
{
	public partial class Dev
	{
		[SlashCommand("ping", "check the current client latency")]
		public async Task Ping()
			=> await Context.Interaction.RespondAsync($"the current ping is {Context.Client.ClientLatency}", ephemeral:true);
	}
}