using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBManager
{
	[AttributeUsage(AttributeTargets.Property)]
	internal class SqlColumnAttribute : Attribute
	{
		public string Name;

		public SqlColumnAttribute(string name)
		{
			Name = name;
		}
	}
}
