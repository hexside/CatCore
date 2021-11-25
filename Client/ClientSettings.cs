using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace Client
{
	internal class ClientSettings
	{
		private static readonly JsonSerializerOptions _options = new()
		{
			AllowTrailingCommas = true,
			IgnoreReadOnlyFields = false,
			IgnoreReadOnlyProperties = false,
			PropertyNameCaseInsensitive = true,
			IncludeFields = true
		};

		public string Token;
		public string WebhookUrl;
		public ulong DebugGuildId;
		public bool DebugMode;
		public string DBConnectionString;
		public ClientSettings(string fileLocation = "clientSettings.jsonc")
		{
			var _this = JsonSerializer.Deserialize<ClientSettings>(File.ReadAllText(fileLocation), _options);

			this.Token = _this.Token.Trim();
			this.WebhookUrl = _this.WebhookUrl.Trim();
			this.DebugGuildId = _this.DebugGuildId;
			this.DebugMode = _this.DebugMode;
			this.DBConnectionString = _this.DBConnectionString;
		}
		public ClientSettings() { }
	}
}
