using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using Discord;
using DBManager;

namespace Client.Commands
{
	[Group("dev", "developer tools, for cool cats only.")]
	public partial class Dev : InteractionModuleBase<SocketInteractionContext>
	{
		[Group("commands", "devtools related to managing commands.")]
		public partial class Commands : InteractionModuleBase<SocketInteractionContext> { }

		[Group("db", "commands related to testing the db")]
		public partial class DB : InteractionModuleBase<SocketInteractionContext> 
		{
			public DBHelper DBHelper { get; set; }
		}
	}
	[Group("pronoun", "command related to managing your pronouns.")]
	public partial class Pronouns : InteractionModuleBase<SocketInteractionContext>
	{
		public DBHelper DBHelper { get; set; }
	}
	public partial class UnGrouped : InteractionModuleBase<SocketInteractionContext> { }
}
