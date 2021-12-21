using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBManager
{
	public class User
	{
		[SqlColumn("isDev")]
		public bool IsDev { get; set; }
		[SqlColumn("discordId")]
		public ulong Id { get; set; }
		[SqlColumn("id")]
		[SqlReadonly]
		public ulong InternalId { get; set; }

		public User(bool isDev, ulong id)
		{
			Id = id;
			IsDev = isDev;
		}

		public User() { }
	}
}