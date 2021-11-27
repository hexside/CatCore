using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBManager
{
	public class UserPronoun
	{
		[SqlColumnName("pronounId")]
		public ulong PronounId { get; set; }
		[SqlColumnName("userId")]
		public ulong UserId { get; set; }
	}
}
