namespace CatCore.Data;

public class CharacterAttribute
{
	public CharacterAttribute()
	{
	}

	public CharacterAttribute(int baseId, int characterId, string value)
	{
		BaseId = baseId;
		CharacterId = characterId;
		Value = value;
	}

	public int CharacterAttributeId { get; set; }

	public int BaseId { get; set; }
	public GuildCharacterAttribute Base { get; set; }

	public int CharacterId { get; set; }
	public Character Character { get; set; }

	public string Value { get; set; }
}
