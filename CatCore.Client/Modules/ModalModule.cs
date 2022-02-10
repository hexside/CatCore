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
