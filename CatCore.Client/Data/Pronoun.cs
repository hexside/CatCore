namespace CatCore.Data;

public class Pronoun
{
	public int PronounId { get; set; }
	
	/// <summary>
	/// 	They went to the park.
	/// </summary>
	public string Subject { get; set; }
	
	/// <summary>
	/// 	I went to the park with them.
	/// </summary>
	public string Object { get; set; }
	
	/// <summary>
	/// 	The park is near their house.
	/// </summary>
	public string PossessiveAdjective { get; set; }
	
	/// <summary>
	/// 	That park is theirs.
	/// </summary>
	public string PossessivePronoun { get; set; }
	
	/// <summary>
	/// 	Someone went to the park by themself.
	/// </summary>
	public string Reflexive { get; set; }
	
	/// <summary>
	/// 	Determines of the string parameters of two pronouns match.
	/// </summary>
	/// <param name="pronoun">The pronoun to compare this one to.</param>
	/// <returns><see langword="true"/> if the pronouns match, otherwise <see langword="false"/></returns>
	public bool Matches(Pronoun pronoun)
		=>  Subject == pronoun.Subject &&
			Object == pronoun.Object &&
			PossessiveAdjective == pronoun.PossessiveAdjective &&
			PossessivePronoun == pronoun.PossessivePronoun &&
			Reflexive == pronoun.Reflexive;

	public override string ToString() => ToString("s/o/a/r");

	/// <summary>
	/// 	replaces: <br/>
	/// 	s => <see cref="Subject"/><br/>
	///  	o => <see cref="Object"/><br/>
	/// 	a => <see cref="PossessiveAdjective"/><br/>
	/// 	p => <see cref="PossessivePronoun"/><br/>
	///		r => <see cref="Reflexive"/>
	/// </summary>
	/// <param name="format">The formatting code</param>
	/// <returns></returns>
	public string ToString(string format)
	{
		format = format
			.Replace("s", "{0}")
			.Replace("o", "{1}")
			.Replace("a", "{2}")
			.Replace("p", "{3}")
			.Replace("r", "{4}");

		return string.Format(format, Subject, Object, PossessiveAdjective, PossessivePronoun, Reflexive);
	}
}
