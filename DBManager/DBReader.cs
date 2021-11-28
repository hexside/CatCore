using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using MySql.Data.MySqlClient;
using System.Text.Json;

namespace DBManager;

/// <summary>
/// A helper class used to convert sql statments to managed objects
/// </summary>
/// <typeparam name="T">The type of object this class will convert</typeparam>
public class DBReader<T>
{

	private static Dictionary<string, PropertyInfo> _map = new();
	private readonly string _sql;
	private readonly MySqlParameter[] _parameters;
	private readonly DBHelper _dbHelper;

	/// <summary>
	///		Initilises the Query
	/// </summary>
	/// <remarks>
	///		When running with the queryType of <see cref="ReadAction.Table"/>
	///		you can specify <see cref="MySqlParameter"/>s via <paramref name="options"/>
	///		each option should have a <see cref="MySqlParameter.ParameterName"/>
	///		that matches the column you want to compare against
	/// </remarks>
	/// <param name="helper">the DBHelper to interact with the databse with</param>
	/// <param name="query">the sql query to run</param>
	/// <param name="queryType">the type of query to run</param>
	/// <param name="options">the parameters to apply to the query</param>
	public DBReader(DBHelper helper, string query, ReadAction queryType, params MySqlParameter[] options)
	{
		_dbHelper = helper;
		_sql = queryType switch
		{
			ReadAction.Embedded => helper.Sql[query],
			ReadAction.RawSql => query,
			ReadAction.Table => $"Select * from {query}",
			_ => throw new ArgumentException("Invalid enum value", nameof(queryType))
		};

		if (queryType == ReadAction.Table && options.Length >= 1)
		{
			_sql += " where " + 
				string.Concat(options.Select(x => $"{x.ParameterName.Cp()}=" +
				$"@{x.ParameterName.Cp()} and "))[..^4] + ";";
		}

		Console.WriteLine(_sql);

		_parameters = options;

		if (_map.Count >= 1)
		{
			_map = _readMap();
		}
	}

	private static Dictionary<string, PropertyInfo> _readMap() => typeof(T)
		.GetProperties()
		.Where(x => x.CanWrite)
		.Where(x => x.GetCustomAttribute<SqlColumnAttribute>() != null)
		.ToDictionary(x => x.GetCustomAttribute<SqlColumnAttribute>().Name.ToLower(),x => x);

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
		Console.WriteLine(_sql);
		MySqlDataReader reader = await _dbHelper.RunQueryAsync(_sql, _parameters);
		List<T> result = ParseReader(reader, tBase);
		reader.Dispose();
		return result;
	}

	/// <summary>
	///		Run the query.
	/// </summary>
	/// <param name="tBase">
	/// The default object to use; set to <see langword="new"/>(<see cref="T"/>)
	/// for a blank implemetation
	/// </param>
	/// <returns>A <see cref="List"/> of the items returned by the query.</returns>
	public List<T> Run(T tBase)
		=> RunAsync(tBase).Result;

	/// <summary>
	///		This method parses a <see cref="MySqlDataReader"/> and produces a 
	///		<see cref="List{T}"/> of staticly typed objects as provided by the
	///		reader.
	/// </summary>
	/// <param name="reader">the reader to parse</param>
	/// <param name="tBase">
	///		The default object to use; set to <see langword="new"/>(<see cref="T"/>)
	///		for a blank implemetation
	/// </param>
	/// <returns>
	///		a <see cref="List{T}"/> grabbed from the reader
	/// </returns>
	public static List<T> ParseReader(MySqlDataReader reader, T tBase)
	{
		_map = _map.Count >= 1
			? _map
			: _readMap();

		List<T> result = new();

		for (int i = 0; reader.Read(); i++)
		{
			result.Add(tBase.Copy());
			for (int j = 0; j < reader.FieldCount; j++)
			{
				if (_map.GetValueOrDefault(reader.GetName(j).ToLower(), null) is PropertyInfo info)
				{
					info.SetValue(result[i], reader[j]);
				}
			}
		}
		return result;
	}
}