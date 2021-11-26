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
		// the name format is "table-column,column" in camelCase
		// param names should be first letter of each word (so columnOne is co)
		// param names should be indexed by adding -n to the end, with n being the
		// zero index parameter number
		//
		// in the case that a parameter needs to be selerated from another; a second -
		// and a susinct description of the diference between the two is good
		// (for example characters-guildId-all and characters-guildId-name)
		private static Dictionary<string, string> _states => new()
		{
			{ "users-discordId", "SELECT * FROM USERS WHERE (discordId=@di);" },
		};

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

		/// <summary>
		///		Runs a myswl Query and returns the rows effected.
		/// </summary>
		/// <param name="sql">the query to run</param>
		/// <param name="options">the options for the query</param>
		/// <returns>the number of rows effected</returns>
		public async Task<int> RunNonQueryAsync(string sql, params MySqlParameter[] options)
			=> await MySqlHelper.ExecuteNonQueryAsync(_connectionString, sql, options);

		/// <summary>
		/// Pings the server
		/// </summary>
		/// <returns>true of the server is avilible, otherwise false</returns>
		public bool Ping()
			=> _connection.Ping();

		/// <summary>
		///		Gets a user from that database
		/// </summary>
		/// <param name="discordId">The users id</param>
		/// <returns>The user</returns>
		public async Task<User> GetUserAsync(ulong discordId)
			=> (await new Query<User>(this, _states["users-discordId"], new MySqlParameter("@di", discordId))
			.RunAsync(new()))
			.First();

	}
}