using System;
using System.Linq;
using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Net;
using Utils;
using System.Reflection;
using System.IO;
using Discord;

namespace DBManager
{
	/// <summary>
	/// A abstrasction layer for <see cref="DBReader{T}"/> and <see cref="DBWriter{T}"/>
	/// that provides easy acess to commen queries, and handles connection pooling
	/// automaticly
	/// </summary>
	public class DBHelper
	{
		private Cached<List<Pronoun>> _pronouns;

		internal string _connectionString;
		/// <summary>
		/// This does not support load balencing and should only be used for pinging the server.
		/// </summary>
		internal MySqlConnection _connection;
		internal Logger _logger;
		/// <summary>
		/// Stores cached versions of embeded sql queries
		/// </summary>
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

			_logger.LogDebug("Caching high value queries").ConfigureAwait(false);
			_pronouns = new(() => GetPronounsAsync().Result);
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
			=> (await new DBReader<User>(this, "users", ReadAction.Table,
				new MySqlParameter("discordId", discordId))
				.RunAsync(new()))
				.FirstOrDefault((User)null);

		/// <summary>
		/// Get a users pronouns
		/// </summary>
		/// <param name="userId">the Id of the user to get pronouns from</param>
		/// <returns>the pronouns the user has specified</returns>
		public async Task<List<Pronoun>> GetPronounsAsync(ulong userId)
			=> await new DBReader<Pronoun>(this, "GetPronounsFromUserId", ReadAction.Embedded,
				new MySqlParameter("userId", userId))
				.RunAsync(new());

		/// <summary>
		/// Gets all pronouns
		/// </summary>
		/// <returns>every pronoun in the db.</returns>
		public async Task<List<Pronoun>> GetPronounsAsync()
			=> _pronouns is null || _pronouns.Valid == false
				? await new DBReader<Pronoun>(this, "pronouns", ReadAction.Table).RunAsync(new())
				: _pronouns.Value;

		/// <summary>
		/// Adds a new user
		/// </summary>
		/// <param name="userId">the users discord id</param>
		/// <param name="isDev">true if the user is a dev, otherwise false</param>
		public async Task AddUserAsync(ulong userId, bool isDev)
			=> await new DBWriter<User>(this, "users", new User(isDev, userId), WriteAction.Add).RunAsync();

		/// <summary>
		/// remove a user
		/// </summary>
		/// <param name="user">the user to remove</param>
		public async Task RemoveUserAsync(User user)
			=> await new DBWriter<User>(this, "users", user, WriteAction.Remove).RunAsync();

		/// <summary>
		/// Adds a pronoun to the database
		/// </summary>
		/// <param name="pronoun">the pronoun to add</param>
		public async Task AddPronounAsync(Pronoun pronoun)
		{
			_pronouns.Valid = false;
			await new DBWriter<Pronoun>(this, "pronouns", pronoun, WriteAction.Add).RunAsync();
		}
		/// <summary>
		/// Checks if a pronoun is a duplicate
		/// </summary>
		/// <param name="pronoun">the pronoun to check</param>
		/// <returns>true if the pronoun is a duplicate; otherwise false</returns>
		public async Task<bool> DuplicatePronoun(Pronoun pronoun)
			 => (await new DBReader<Pronoun>(this, "pronouns", ReadAction.Table,
				new("subject", pronoun.Subject),
				new("object", pronoun.Object),
				new("possesive", pronoun.Possesive),
				new("reflexive", pronoun.Reflexive))
			.RunAsync(new()))
			.Any();

		/// <summary>
		/// Get a pronoun by its id
		/// </summary>
		/// <param name="id">the pronouns id</param>
		/// <returns>the pronoun</returns>
		public async Task<Pronoun> GetPronounAsync(ulong id)
			=> (await new DBReader<Pronoun>(this, "pronouns", ReadAction.Table, new MySqlParameter("id", id))
			.RunAsync(new()))
			.First();

		/// <summary>
		/// Adds a pronoun to a user
		/// </summary>
		/// <param name="pronounId">The id of the pronoun</param>
		/// <param name="userId">The id of the user</param>
		public async Task AddUserPronounAsync(ulong pronounId, ulong userId)
			=> await new DBWriter<UserPronoun>(this, "userPronouns", new(pronounId, userId), WriteAction.Add).RunAsync();

		/// <summary>
		/// Removes a pronoun from a user
		/// </summary>
		/// <param name="pronounId">The id of the pronoun</param>
		/// <param name="userId">The id of the user</param>
		public async Task RemoveUserPronounAsync(ulong pronounId, ulong userId)
			=> await new DBWriter<UserPronoun>(this, "userPronouns", new(pronounId, userId), WriteAction.Remove).RunAsync();

		/// <summary>
		/// Gets a poll from the database
		/// </summary>
		/// <param name="id">the polls id</param>
		/// <returns>The poll.</returns>
		public async Task<Poll> GetPollAsync(ulong id)
			=> (await new DBReader<Poll>(this, "polls", ReadAction.Table, new MySqlParameter("id", id))
				.RunAsync(new()))
				.First();

		/// <summary>
		/// Gets polls from a guild
		/// </summary>
		/// <param name="guildId">the guild to get polls from</param>
		/// <returns>all the polls in the guild</returns>
		public async Task<List<Poll>> GetGuildPollsAsync(ulong guildId)
			=> await new DBReader<Poll>(this, "polls", ReadAction.Table, new MySqlParameter("guildId", guildId))
				.RunAsync(new());

		/// <summary>
		/// Gets the roles from a poll
		/// </summary>
		/// <param name="pollId">the polls id</param>
		/// <returns>all the roles in a poll</returns>
		public async Task<List<PollRole>> GetPollRolesAsync(ulong pollId)
			=> await new DBReader<PollRole>(this, "pollRoles", ReadAction.Table, new MySqlParameter("pollId", pollId))
				.RunAsync(new());

		/// <summary>
		/// gets a poll role
		/// </summary>
		/// <param name="id">the roles id</param>
		/// <returns>the pollrole</returns>
		public async Task<PollRole> GetPollRoleAsync(ulong id)
			=> (await new DBReader<PollRole>(this, "pollRoles", ReadAction.Table, new MySqlParameter("id", id))
				.RunAsync(new()))
				.First();

		/// <summary>
		/// Adds a poll to the db
		/// </summary>
		/// <param name="poll">the poll to add</param>
		public async Task AddPollAsync(Poll poll)
			=> await new DBWriter<Poll>(this, "polls", poll, WriteAction.Add).RunAsync();

		public async Task AddPollRoleAsync(PollRole role)
			=> await new DBWriter<PollRole>(this, "pollRoles", role, WriteAction.Add).RunAsync();
	}
}