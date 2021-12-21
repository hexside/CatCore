using System;

namespace DBManager
{
	public struct ListMatchesResult
	{
		public bool Item1;
		public string Item2;

		public ListMatchesResult(bool item1, string item2)
		{
			Item1 = item1;
			Item2 = item2;
		}

		public override bool Equals(object obj) => obj is ListMatchesResult other && Item1 == other.Item1 && Item2 == other.Item2;
		public override int GetHashCode() => HashCode.Combine(Item1, Item2);

		public void Deconstruct(out bool item1, out string item2)
		{
			item1 = Item1;
			item2 = Item2;
		}

		public static implicit operator (bool, string)(ListMatchesResult value) => (value.Item1, value.Item2);
		public static implicit operator ListMatchesResult((bool, string) value) => new (value.Item1, value.Item2);

		public static bool operator ==(ListMatchesResult left, ListMatchesResult right) => left.Equals(right);

		public static bool operator !=(ListMatchesResult left, ListMatchesResult right) => !(left == right);
	}
}
