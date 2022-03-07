namespace CatCore.Data;

public class CatCoreDbContext : DbContext
{
	public string DbPath { get; set; }

	public DbSet<Poll> Polls { get; set; }
	public DbSet<PollRole> PollRoles { get; set; }
	public DbSet<Pronoun> Pronouns { get; set; }
	public DbSet<User> Users { get; set; }
	public DbSet<Message> Messages { get; set; }
	public DbSet<UserMessage> UserMessages { get; set; }
	public DbSet<MessageGroup> MessageGroups { get; set; }
	public DbSet<Guild> Guilds { get; set; }
	public DbSet<RegexAction> RegexActions { get; set; }
	public DbSet<Character> Characters { get; set; }
	public DbSet<GuildCharacterAttribute> GuildCharacterAttributes { get; set; }
	public DbSet<CharacterAttribute> CharacterAttributes { get; set; }

	public CatCoreDbContext(string dbPath = "CatCore.db")
	{
		DbPath = dbPath;
	}

	protected override void OnConfiguring(DbContextOptionsBuilder builder)
	{
		builder.UseSqlite($"Data Source={DbPath}");
	}
}
