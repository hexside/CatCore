using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using MySql.Data.MySqlClient;

namespace DBManager
{
	public class Query<T>
	{
		private static Dictionary<Type, Dictionary<string, PropertyInfo>> _map = new();
		private readonly string _sql;
		private readonly MySqlParameter[] _parameters;
		private readonly DBHelper _dbHelper;

		public Query(DBHelper helper, string sql, params MySqlParameter[] options)
		{
			var s = typeof(T).GetFields().SelectMany(x => x.GetCustomAttributesData());
			var s2 = typeof(T).GetFields().SelectMany(x => x.GetCustomAttributes());
			var s3 = typeof(T).GetFields().SelectMany(x => x.GetCustomAttributes<SqlColumnNameAttribute>());
			var others = typeof(T);
			_dbHelper = helper;
			_sql = sql;
			_parameters = options;

			if (!_map.ContainsKey(typeof(T)))
			{
				_map.Add(typeof(T), _readMap());
			}
		}

		private static Dictionary<string, PropertyInfo> _readMap() => typeof(T)
			.GetProperties()
			.Where(x => x.CanWrite)
			.Where(x => x.GetCustomAttribute<SqlColumnNameAttribute>() != null)
			.ToDictionary(x => x.GetCustomAttribute<SqlColumnNameAttribute>().Name, x => x);

		public static void DumbMappings()
			=> _map = new();

		public T Run(T value)
		{
			Dictionary<string, PropertyInfo> map = _map.GetValueOrDefault(typeof(T), null)
				?? _readMap();

			MySqlDataReader reader = _dbHelper.RunQueryAsync(_sql, _parameters).Result;

			while (reader.Read())
			{
				for (int i = 0; i < reader.FieldCount; i++)
				{
					// if a setter for the specified row exists use it
					if (map.GetValueOrDefault(reader.GetName(i), null) is PropertyInfo info)
					{
						info.SetValue(value, reader[i]);
					}
				}
			}

			reader.Dispose();
			return value;
		}
	}
}
