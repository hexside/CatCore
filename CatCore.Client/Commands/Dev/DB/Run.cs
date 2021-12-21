using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Interactions;
using System.Text.Json;
using Discord;
using DBManager;

namespace Client.Commands
{
	public partial class Dev
	{
		public partial class DB
		{
			[SlashCommand("run", "Run raw sql code in the client")]
			public async Task RQ(
			[Summary("command", "the sql command that will be run")]
			string command, 
			[Summary("responce-type", "the type of result the command will have")]
			ResponceForm responceType, 
			[Summary("type", "the type of object to convert the reader to")]
			SqlResultType type = SqlResultType.Invalid)
			{
				await Context.Interaction.DeferAsync(true);
				string responce = responceType != ResponceForm.ConvertedReader
					? (await DBHelper.RunNonQueryAsync(command)).ToString() + " Rows Effected."
					: type switch
					{
						// use a fancy json conversion if possible
						SqlResultType.User => JsonSerializer.Serialize<List<User>>(
							new DBReader<User>(DBHelper, command, ReadAction.RawSql).Run(new()), _jsonOptions),
						SqlResultType.Pronoun => JsonSerializer.Serialize<List<DBManager.Pronoun>>(
							new DBReader<DBManager.Pronoun>(DBHelper, command, ReadAction.RawSql).Run(new()), _jsonOptions),
						SqlResultType.Poll => JsonSerializer.Serialize<List<Poll>>(
							new DBReader<Poll>(DBHelper, command, ReadAction.RawSql).Run(new()), _jsonOptions),
						SqlResultType.PollRole => JsonSerializer.Serialize<List<PollRole>>(
							new DBReader<PollRole>(DBHelper, command, ReadAction.RawSql).Run(new()), _jsonOptions),
						// when that fails fallback on the legacy behavior
						_ => (await DBHelper.RunQueryAsync(command)).ReadAsText(2000)
					};
				responce = responce.Length > 1985
					? responce[..1985]
					: responce;
				await Context.Interaction.FollowupAsync($"```json\n{responce}\n```");
			}

			public enum SqlResultType
			{
				User,
				Pronoun,
				Poll,
				PollRole,
				Invalid
			}
		}
	}
}