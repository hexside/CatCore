using System.Text.Json;

namespace CatCore.Client
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

			Token = _this.Token.Trim();
			WebhookUrl = _this.WebhookUrl.Trim();
			DebugGuildId = _this.DebugGuildId;
			DebugMode = _this.DebugMode;
			DBConnectionString = _this.DBConnectionString;
		}
		public ClientSettings() { }
	}
}
