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
						SqlResultType.User => JsonSerializer.Serialize(
							new Query<User>(DBHelper, command, QueryType.RawSql).Run(new()), _jsonOptions),
						SqlResultType.Pronoun => JsonSerializer.Serialize(
							new Query<Pronoun>(DBHelper, command, QueryType.RawSql).Run(new()), _jsonOptions),
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
				Invalid
			}
		}
	}
}