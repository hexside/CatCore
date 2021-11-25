using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Ubiety.Dns.Core;

namespace DBManager
{
	public static class DBExtentions
	{
		/// <summary>
		///		Get the content of a <see cref="MySqlDataReader"/> as a string, 
		///		then dispose of the reader.
		/// </summary>
		/// <param name="reader">The <see cref="MySqlDataReader"/> to get content from.</param>
		/// <param name="maxLength">The length at which the string will be truanced.</param>
		/// <returns>A string representing the readers content.</returns>
		public static string ReadAsText(this MySqlDataReader reader, int maxLength)
		{
			string responce = "";
			while (reader.Read())
			{
				// write all fields to output
				for (int i = 0; i < reader.FieldCount; i++)
				{
					responce += reader[i].ToString() + "; ";
				}

				// add newline and remove trailing "; "
				responce = responce[..^2] + "\n";

				// account for max message length
				if (responce.Length > maxLength)
				{
					// added four dots because one will be removed when the value is returned.
					responce = responce[..(maxLength - 3)] + "....";
					break;
				}
			}
			
			// return a value accounting for the added newline
			reader.Dispose();
			return responce[..^1];
		}
	}
}
