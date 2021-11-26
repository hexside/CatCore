using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using MySql.Data.MySqlClient;

namespace DBManager
{
	/// <summary>
	/// A helper class used to convert sql statments to managed objects
	/// </summary>
	/// <typeparam name="T">The type of object this class will convert</typeparam>
	public class Query<T>
	{
		private static Dictionary<string, PropertyInfo> _map = new();
		private readonly string _sql;
		private readonly MySqlParameter[] _parameters;
		private readonly DBHelper _dbHelper;

		/// <summary>
		/// Initilises the Query
		/// </summary>
		/// <param name="helper">the DBHelper to interact with the databse with</param>
		/// <param name="sql">the sql query to run</param>
		/// <param name="options">the parameters to apply to the query</param>
		public Query(DBHelper helper, string sql, params MySqlParameter[] options)
		{
			_dbHelper = helper;
			_sql = sql;
			_parameters = options;

			if (_map.Count >= 1)
			{
				_map = _readMap();
			}
		}

		private static Dictionary<string, PropertyInfo> _readMap() => typeof(T)
			.GetProperties()
			.Where(x => x.CanWrite)
			.Where(x => x.GetCustomAttribute<SqlColumnNameAttribute>() != null)
			.ToDictionary(x => x.GetCustomAttribute<SqlColumnNameAttribute>().Name.ToLower(),x => x);

		/// <summary>
		///		Clear the map cache on <see cref="T"/>
		/// </summary>
		public static void DumbMappings()
			=> _map = new();

		/// <summary>
		///		Run the query.
		/// </summary>
		/// <param name="tBase">
		/// The default object to use; set to <see langword="new"/>(<see cref="T"/>)
		/// for a blank implemetation
		/// </param>
		/// <returns>A <see cref="List"/> of the items returned by the query.</returns>
		public async Task<List<T>> RunAsync(T tBase)
		{
			_map = _map.Count >= 1 
				? _map 
				: _readMap();

			MySqlDataReader reader =  await _dbHelper.RunQueryAsync(_sql, _parameters);
			List<T> result = new();

			for (int i = 0; reader.Read(); i++)
			{
				result.Add(tBase);
				for (int j = 0; j < reader.FieldCount; j++)
				{
					if (_map.GetValueOrDefault(reader.GetName(j).ToLower(), null) is PropertyInfo info)
					{
						info.SetValue(result[i], reader[j]);
					}
				}
			}

			reader.Dispose();
			return result;
		}
	}
}
