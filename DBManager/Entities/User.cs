using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBManager
{
	public class User
	{
		[SqlColumnName("isDev")]
		public bool IsDev { get; set; }
		[SqlColumnName("discordId")]
		public ulong Id { get; set; }
		[SqlColumnName("id")]
		public ulong InternalId { get; set; }
	}
}