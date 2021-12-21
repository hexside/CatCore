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
		[SqlReadonly]
		[SqlColumn("id")]
		public ulong Id { get; set; }

		[SqlColumn("pollId")]
		public ulong PollId { get; set; }

		[SqlColumn("roleId")]
		public ulong RoleId { get; set; }

		[SqlColumn("description")]
		public string Description { get; set; }
	}
}
