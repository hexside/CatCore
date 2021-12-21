using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatCore.Utils
{
	public class Cached<T>
	{
		public T _value;

		/// <summary>
		///		Set this value to <see langword="false"/> to rebuild it
		///		next time it is acessed
		/// </summary>
		public bool Valid;
		/// <summary>
		///		When this cached item will expire
		/// </summary>
		public DateTime Expires;
		/// <summary>
		///		How long the object is valid for after being rebuild
		/// </summary>
		public TimeSpan DefaultValidTime;

		/// <summary>
		///		The actual cached object
		/// </summary>
		public T Value 
		{
			get => _validateOrCreate();
			set
			{
				_value = value;
				Expires = DateTime.Now.Add(DefaultValidTime);
			}
		}

		/// <summary>
		///		Gets a new instance of <see cref="Value"/> after it expires or is invalidated
		/// </summary>
		public Func<T> GetNew;

		private T _validateOrCreate()
		{
			if (!Valid || DateTime.Compare(Expires, DateTime.Now) <= 0)
			{
				Valid = false;
				_value = GetNew();
				Expires = DateTime.Now.Add(DefaultValidTime);
				Valid = true;
			}
			return _value;
		}

		/// <summary>
		///		Creates a new instance of a <see cref="Cached{T}"/>
		/// </summary>
		/// <param name="restore">
		///		a <see cref="Func{T}"/> that can be called to get a valid verison of <paramref name="value"/>
		///		when it is invalidated
		///	</param>
		/// <param name="validFor">how long a cached object is valud for</param>
		public Cached (Func<T> restore, TimeSpan? validFor = null)
		{
			DefaultValidTime = validFor ?? new(0, 5, 0);
			Valid = true;
			GetNew = restore;
			Expires = DateTime.Now.Add(DefaultValidTime);
			_value = restore.Invoke();
		}
	}
}
