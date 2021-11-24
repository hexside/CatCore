using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord;

namespace Client.Commands
{
	[Group("dev", "developer tools, for cool cats only.")]
	public partial class Dev : InteractionModuleBase<SocketInteractionContext>
	{
		[Group("commands", "devtools related to managing commands.")]
		public partial class Commands : InteractionModuleBase<SocketInteractionContext> { }
	}

	public partial class UnGrouped 
	{

	}
}
