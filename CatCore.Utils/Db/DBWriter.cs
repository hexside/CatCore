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
	private Dictionary<PropertyInfo, string> _map = new();

	private readonly DBHelper _helper;
	private readonly string _sql;
	private readonly List<MySqlParameter> _parameters;
	private readonly List<MySqlParameter> _nParameters;
	private readonly T _obj;
	private readonly T _nObj;
	private readonly WriteAction _action;
	private readonly string _table;

	public DBWriter(DBHelper helper, string table, T obj, WriteAction action)
	{
		if (action == WriteAction.Update) throw new ArgumentException("nObj must be specified to use WriteAction.Update");
		_helper = helper;
		_obj = obj;
		_action = action;
		_table = table;
		_validateMap(true);
		_parameters = _readParameters();
		_sql = _readSql();
	}

	public DBWriter(DBHelper helper, string table, T obj, WriteAction action, T nObj)
	{
		_helper = helper;
		_obj = obj;
		_nObj = nObj;
		_action = action;
		_table = table;
		_validateMap(false);
		_parameters = _readParameters();
		_nParameters = _readNParameters();
		_sql = _readSql();
	}

	private static Dictionary<PropertyInfo, string> _readMap(bool blockReadOnly) => typeof(T)
		.GetProperties()
		.Where(x => x.CanWrite)
		.Where(x => x.GetCustomAttribute<SqlColumnAttribute>() != null)
		.Where(x => x.GetCustomAttribute<SqlReadonlyAttribute>() == null || !blockReadOnly)
		.ToDictionary(x => x, x => x.GetCustomAttribute<SqlColumnAttribute>().Name.ToLower());

	private List<MySqlParameter> _readParameters()
		=> _map.Select(x => new MySqlParameter(x.Value, x.Key.GetValue(_obj))).ToList();

	private List<MySqlParameter> _readNParameters()
		=> _map.Select(x => new MySqlParameter("n" + x.Value, x.Key.GetValue(_nObj))).ToList();

	private void _validateMap(bool blockReadOnly)
		=> _map = _map.Count < 1
			? _readMap(blockReadOnly)
			: _map;

	private string _readSql() => _action switch
	{
		WriteAction.Add => $"INSERT INTO {_table}" +
			$"({string.Concat(_parameters.Select(x => x.ParameterName.Cp() + ", "))[..^2]}) value " +
			$"({string.Concat(_parameters.Select(x => $"@{x.ParameterName.Cp()}, "))[..^2]});",
		WriteAction.Remove => $"DELETE FROM {_table} where (" + string.Concat(_parameters.Select(x =>
				$"{x.ParameterName.Cp()}=@{x.ParameterName.Cp()} = "))[..^3] + ")",
		WriteAction.Update => $@"UPDATE {_table} SET {string.Concat(_nParameters.Select(x =>
			$@"{x.ParameterName.Cp()[1..]}=@{x.ParameterName.Cp()} and "))[..^5]} where (" + string.Concat(_parameters.Select(x =>
			$@"{x.ParameterName.Cp()}=@{x.ParameterName.Cp()} and "))[..^5] + ")",
		_ => throw new NotImplementedException()
	};

	/// <summary>
	/// Runs this query.
	/// </summary>
	/// <returns>the number of effected rows</returns>
	public async Task<int> RunAsync()
	{
		if (_nParameters?.Count >= 1) _parameters.AddRange(_nParameters);
		MySqlParameter[] parameters = _parameters.ToArray();
		await _helper._logger.LogDebug($"Running the query {_sql}\n{parameters.GetString()}").ConfigureAwait(false);
		return await _helper.RunNonQueryAsync(_sql, parameters);
	}

	/// <summary>
	/// Runs this query.
	/// </summary>
	/// <returns>the number of effected rows</returns>
	public int Run()
		=> RunAsync().Result;

	public void LogContent() => Console.WriteLine(_sql);
}
