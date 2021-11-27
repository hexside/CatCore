using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBManager
{
	public class UserPronoun
	{
		[SqlReadonly]
		[SqlColumn("pronounId")]
		public ulong PronounId { get; set; }
		[SqlColumn("userId")]
		public ulong UserId { get; set; }
	}
}
