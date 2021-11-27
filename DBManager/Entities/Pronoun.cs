﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DBManager
{
	public class Pronoun
	{
		[SqlColumnName("id")]
		public ulong Id { get; set; }
		private string _subject;
		[SqlColumnName("subject")]
		public string Subject
		{
			get => _subject;
			set => _subject = value.Length < 50
				? value
				: throw new ArgumentException("cannot write to _subject because value is to long");
		}
		private string _object;
		[SqlColumnName("object")]
		public string Object
		{
			get => _object;
			set => _object = value.Length < 50
				? value
				: throw new ArgumentException("cannot write to _object because value is to long");
		}
		private string _possesive;
		[SqlColumnName("possesive")]
		public string Possesive
		{
			get => _possesive;
			set => _possesive = value.Length < 50
				? value
				: throw new ArgumentException("cannot write to _possesive because value is to long");
		}
		private string _reflexive;
		[SqlColumnName("reflexive")]
		public string Reflexive
		{
			get => _reflexive;
			set => _reflexive = value.Length < 50
				? value
				: throw new ArgumentException("cannot write to _possesive because value is to long");
		}

		public override string ToString() => $"{_subject}/{_object}/{_possesive}/{_reflexive}";
	}
}