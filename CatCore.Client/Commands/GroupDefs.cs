namespace CatCore.Client.Commands
{
	[Group("dev", "developer tools, for cool cats only.")]
	public partial class Dev : InteractionModuleBase<SocketInteractionContext>
	{
		[Group("commands", "devtools related to managing commands.")]
		public partial class Commands : InteractionModuleBase<SocketInteractionContext> { }
	}
}
