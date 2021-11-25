using System;
using System.Linq;
using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Net;

namespace DBManager
{
	public class DBHelper
	{
		internal string _connectionString;
		/// <summary>
		/// This does not support load balencing and should only be used for pinging the server.
		/// </summary>
		internal MySqlConnection _connection;

		public DBHelper(string connectionString)
		{
			_connection = new MySqlConnection(connectionString);
			_connection.Open();
			// validate the string
			if (!_connection.Ping())
				throw new ArgumentException("The databse is ofline or the connection string is invalid", 
					nameof(connectionString));

			_connectionString = connectionString;
		}

		/// <summary>
		/// Runs a mysql query on the server.
		/// </summary>
		/// <param name="sql">the query to run</param>
		/// <param name="options">the options for the query</param>
		/// <returns>a <see cref="MySqlDataReader"/> representing the query output</returns>
		public async Task<MySqlDataReader> RunQueryAsync(string sql, params MySqlParameter[] options)
			=> await MySqlHelper.ExecuteReaderAsync(_connectionString, sql, options);

		public async Task<int> RunNonQueryAsync(string sql, params MySqlParameter[] options)
			=> await MySqlHelper.ExecuteNonQueryAsync(_connectionString, sql, options);

		/// <summary>
		/// Pings the server
		/// </summary>
		/// <returns>true of the server is avilible, otherwise false</returns>
		public bool Ping()
			=> _connection.Ping();
	}
}