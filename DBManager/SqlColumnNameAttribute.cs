using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBManager
{
	[AttributeUsage(AttributeTargets.Property)]
	internal class SqlColumnNameAttribute : Attribute
	{
		public string Name;

		public SqlColumnNameAttribute(string name)
		{
			Name = name;
		}
	}
}
