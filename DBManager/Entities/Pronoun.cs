using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DBManager
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
		private string _possesive = "";
		[SqlColumn("possesive")]
		public string Possesive
		{
			get => _possesive;
			set => _possesive = value.Length < 50
				? value
				: throw new ArgumentException("cannot write to _possesive because value is to long");
		}
		private string _reflexive = "";
		[SqlColumn("reflexive")]
		public string Reflexive
		{
			get => _reflexive;
			set => _reflexive = value.Length < 50
				? value
				: throw new ArgumentException("cannot write to _possesive because value is to long");
		}

		public override string ToString() => $"{_subject}/{_object}/{_possesive}/{_reflexive}";

		public string ToString(string format, params object[] args)
		{
			for (int i = 0; i < args.Length; i++) format = format.Replace($"{{{i}}}", args[i].ToString());
			format = format
				.Replace("s", _subject)
				.Replace("o", _object)
				.Replace("p", _possesive)
				.Replace("r", _reflexive);
			return format;
		}
	}
}
