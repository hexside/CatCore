using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace CatCore.Data
{
	public class PollRole
	{
		public int PollRoleId { get; set; }
		public ulong RoleId { get; set; }
		public string Description { get; set; }
		public int PollId { get; set; }
		public Poll Poll { get; set; }
		public string Emote { get; set; }
	}
}
