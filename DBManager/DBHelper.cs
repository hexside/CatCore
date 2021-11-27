using System;
using System.Linq;
using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Net;
using CottageDwellingAdditions;
using System.Reflection;
using System.IO;
using Discord;

namespace DBManager
{
	public class DBHelper
	{
		internal string _connectionString;
		/// <summary>
		/// This does not support load balencing and should only be used for pinging the server.
		/// </summary>
		internal MySqlConnection _connection;
		internal Logger _logger;
		public Dictionary<string, string> Sql { get; private set; }
		
		public event Func<LogMessage, Task> Log;

		public DBHelper(string connectionString)
		{
			_logger = new("dbHelper", LogSeverity.Debug);
			_logger.LogFired += async x => await Log.Invoke(x);
			_connectionString = connectionString;

			_connection = new MySqlConnection(connectionString);
			_connection.Open();
			// validate the string
			if (!_connection.Ping())
			{
				string error = "The databse is ofline or the connection string is invalid";
				throw new ArgumentException(error, nameof(connectionString));
			}
		}

		/// <summary>
		/// Rebuilds the cache of sql queries stored.
		/// </summary>
		public void Init()
		{
			_logger.LogVerbose("starting db init process").ConfigureAwait(false);
			Sql = new();
			Assembly assembly = Assembly.GetExecutingAssembly();
			List<string> sqlFiles = assembly.GetManifestResourceNames()
				.Where(x => x.EndsWith(".sql", StringComparison.InvariantCultureIgnoreCase))
				.ToList();
			_logger.LogVerbose($"found {sqlFiles.Count} embedded sql files.").ConfigureAwait(false);

			sqlFiles.OnEach(x =>
			{
				string cleanName = string.Concat(x.Split('.')[^2..^1]);
				Stream s = assembly.GetManifestResourceStream(x);
				using StreamReader reader = new(s);
				_logger.LogDebug($"adding the sql file {cleanName}").ConfigureAwait(false);
				Sql.Add(cleanName, reader.ReadToEnd());
			});
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
		/// Runs a mysql query on the server.
		/// </summary>
		/// <param name="sql">the query to run</param>
		/// <param name="options">the options for the query</param>
		/// <returns>a <see cref="MySqlDataReader"/> representing the query output</returns>
		public MySqlDataReader RunQuery(string sql, params MySqlParameter[] options)
			=> MySqlHelper.ExecuteReader(_connectionString, sql, options);

		/// <summary>
		///		Runs a myswl Query and returns the rows effected.
		/// </summary>
		/// <param name="sql">the query to run</param>
		/// <param name="options">the options for the query</param>
		/// <returns>the number of rows effected</returns>
		public int RunNonQuery(string sql, params MySqlParameter[] options)
			=> MySqlHelper.ExecuteNonQuery(_connectionString, sql, options);

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
			=> (await new Query<User>(this, "GetUserFromDiscordId", QueryType.Embedded,
				new MySqlParameter("@discordId", discordId))
				.RunAsync(new()))
				.First();

		/// <summary>
		/// Get a users pronouns
		/// </summary>
		/// <param name="userId">the Id of the user to get pronouns from</param>
		/// <returns>the pronouns the user has specified</returns>
		public async Task<List<Pronoun>> GetPronounsAsync(ulong userId)
			=> await new Query<Pronoun>(this, "GetPronounsFromUserId", QueryType.Embedded,
				new MySqlParameter("@userId", userId))
				.RunAsync(new());
	}
}