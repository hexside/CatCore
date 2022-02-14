namespace CatCore.Client.Modals;

public class PollModal : IModal
{
	public string Title => "Poll";
	
	[ModalTextInput("name", TextInputStyle.Short, "Enter your poll's name", 1, 256)]
	[InputLabel("Name")]
	public string Name { get; set; }
	
	[ModalTextInput("description", TextInputStyle.Paragraph, "Enter your poll's description", 1, 2048)]
	[InputLabel("Description")]
	public string Description { get; set; }
	
	[ModalTextInput("footer", TextInputStyle.Paragraph, "Enter your poll's footer", 1, 2048)]
	[InputLabel("Footer")]
	public string Footer { get; set; }

	[ModalTextInput("image_url", TextInputStyle.Short, "Enter your poll's image url", 1, 1024)]
	[InputLabel("image url")]
	[RequiredInput(false)]
	public string? ImageUrl { get; set; }
}

public class AutomodRegexInputModal : IModal
{
	public string Title => "Regex Filter";

	[ModalTextInput("name", TextInputStyle.Short, "Enter the filter's name", 1, 25)]
	[InputLabel("Name")]
	public string Name { get; set; }
	
	[ModalTextInput("filter", TextInputStyle.Paragraph, "Enter the Regex filter. (non-recursive, timed out at 10 ms)", 
		1, 250)]
	[InputLabel("Filter")]
	public string Filter { get; set; }
}

public class AutomodInputModal : IModal
{
	public string Title => "Automod Filter";

	[ModalTextInput("name", TextInputStyle.Short, "Enter the filter's name", 1, 25)]
	[InputLabel("Name")]
	public string Name { get; set; }
	
	[ModalTextInput("filter", TextInputStyle.Paragraph, "Enter the filter to check for", 1, 250)]
	[InputLabel("Filter")]
	public string Filter { get; set; }
}

public class AutomodTestModal : IModal
{
	public string Title => "Automod Test";

	[ModalTextInput("message", TextInputStyle.Paragraph, "Enter the message to test", 1, 4000)]
	[InputLabel("message")]
	public string Message { get; set; }
}
