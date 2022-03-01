namespace CatCore.Data;

public class GuildCharacterAttribute
{
	public int GuildCharacterAttributeId { get; set; }

	public string Name { get; set; }
	public string Description { get; set; }
	public bool Required { get; set; }
	public bool Valid { get; set; }

	public string? RegexValidator { get; set; }
	public int? MinValue { get; set; }
	public int? MaxValue { get; set; } = 1000;
	public bool? Multiline { get; set; }
}
