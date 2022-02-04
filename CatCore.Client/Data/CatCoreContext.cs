namespace CatCore.Data;

public class CatCoreContext : DbContext
{
	public string DbPath { get; set; }
	
	public DbSet<Poll> Polls { get; set; }
	public DbSet<PollRole> PollRoles { get; set; }
	public DbSet<Pronoun> Pronouns { get; set; }
	public DbSet<User> Users { get; set; }
	public DbSet<Message> Messages { get; set; }
	public DbSet<UserMessage> UserMessages { get; set; }

	public CatCoreContext(string dbPath = "CatCore.db")
	{
		DbPath = dbPath;
	}
	
	protected override void OnConfiguring(DbContextOptionsBuilder builder)
	{
		builder.UseSqlite($"Data Source={DbPath}");
	}
}
