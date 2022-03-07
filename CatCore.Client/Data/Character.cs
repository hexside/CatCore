namespace CatCore.Data;

public class Character
{
	public int CharacterId { get; set; }

	public string Name { get; set; }
	public string Description { get; set; }
	public List<CharacterAttribute> Attributes { get; set; } = new();
	public uint Color { get; set; }
	public string? ImageUrl { get; set; }

	public int CreatorId { get; set; }
	public User Creator { get; set; }

	public EmbedBuilder GetEmbed()
		=> new EmbedBuilder()
			.WithThumbnailUrl(ImageUrl)
			.WithDescription(Description)
			.WithTitle(Name)
			.WithFields(Attributes.Select(x => new EmbedFieldBuilder() { Name = x.Base.Name, Value = x.Value }));
}
