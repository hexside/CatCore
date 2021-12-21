using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CatCore.Data
{
	public class Pronoun
	{
		[SqlReadonly]
		[SqlColumn("id")]
		public ulong Id { get; set; }
		private string _subject = "";
		[SqlColumn("subject")]
		public string Subject
		{
			get => _subject;
			set => _subject = value.Length < 50
				? value
				: throw new ArgumentException("cannot write to _subject because value is to long");
		}
		private string _object = "";
		[SqlColumn("object")]
		public string Object
		{
			get => _object;
			set => _object = value.Length < 50
				? value
				: throw new ArgumentException("cannot write to _object because value is to long");
		}
		private string _possessive = "";
		[SqlColumn("possessive")]
		public string Possessive
		{
			get => _possessive;
			set => _possessive = value.Length < 50
				? value
				: throw new ArgumentException("cannot write to _possessive because value is to long");
		}
		private string _reflexive = "";
		[SqlColumn("reflexive")]
		public string Reflexive
		{
			get => _reflexive;
			set => _reflexive = value.Length < 50
				? value
				: throw new ArgumentException("cannot write to _reflexive because value is to long");
		}

		public override string ToString() => $"{_subject}/{_object}/{_possessive}/{_reflexive}";

		public string ToString(string format)
		{
			format = format
				.Replace("s", "{0}")
				.Replace("o", "{1}")
				.Replace("p", "{2}")
				.Replace("r", "{3}");
				
			return string.Format(format, _subject, _object, _possessive, _reflexive);
		}
	}
}
