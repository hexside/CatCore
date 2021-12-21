using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using MySql.Data.MySqlClient;

namespace CatCore.Data;

/// <summary>
///		A helper class used convert to manged objects to sql statements, 
///		and add them to the database
/// </summary>
/// <typeparam name="T">The type of object this class will convert</typeparam>
public class DBWriter<T>
{
	private static Dictionary<PropertyInfo, string> _map = new();

	private readonly DBHelper _helper;
	private readonly string _sql;
	private readonly List<MySqlParameter> _parameters;
	private readonly T _obj;
	private readonly WriteAction _action;
	private readonly string _table;

	public DBWriter(DBHelper helper, string table, T obj, WriteAction action)
	{
		_helper = helper;
		_obj = obj;
		_action = action;
		_table = table;
		_validateMap();
		_parameters = _readParameters();
		_sql = _readSql();
		_helper._logger.LogDebug($"Running the query {_sql}\n{_parameters.GetString()}").ConfigureAwait(false);
	}

	private static Dictionary<PropertyInfo, string> _readMap() => typeof(T)
		.GetProperties()
		.Where(x => x.CanWrite)
		.Where(x => x.GetCustomAttribute<SqlColumnAttribute>() != null)
		.Where(x => x.GetCustomAttribute<SqlReadonlyAttribute>() == null)
		.ToDictionary(x => x, x => x.GetCustomAttribute<SqlColumnAttribute>().Name.ToLower());

	private List<MySqlParameter> _readParameters()
		=> _map.Select(x => new MySqlParameter(x.Value, x.Key.GetValue(_obj))).ToList();

	private static void _validateMap()
		=> _map = _map.Count < 1
			? _readMap()
			: _map;

	private string _readSql() => _action switch
	{
		WriteAction.Add => $"INSERT INTO {_table}" +
			$"({string.Concat(_parameters.Select(x => x.ParameterName.Cp() + ", "))[..^2]}) value " +
			$"({string.Concat(_parameters.Select(x => $"@{x.ParameterName.Cp()}, "))[..^2]});",
		WriteAction.Remove => $"DELETE FROM {_table} where (" + string.Concat(_parameters.Select(x =>
				$"{x.ParameterName.Cp()}=@{x.ParameterName.Cp()} and "))[..^5] + ")",
		_ => throw new NotImplementedException()
	};

	/// <summary>
	/// Runs this query.
	/// </summary>
	/// <returns>the number of effected rows</returns>
	public async Task<int> RunAsync()
		=> await _helper.RunNonQueryAsync(_sql, _parameters.ToArray());

	/// <summary>
	/// Runs this query.
	/// </summary>
	/// <returns>the number of effected rows</returns>
	public int Run()
		=> RunAsync().Result;

	public void LogContent() => Console.WriteLine(_sql);
}
