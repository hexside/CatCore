using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBManager
{
	[AttributeUsage(AttributeTargets.Property)]
	internal class SqlReadonlyAttribute : Attribute
	{
		public bool set = false;

		public SqlReadonlyAttribute()
		{
			set = true;
		}
	}
}
