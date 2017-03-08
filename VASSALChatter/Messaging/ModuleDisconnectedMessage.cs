using System;
namespace VASSALChatter
{
	public class ModuleDisconnectedMessage : IMessage
	{
		public string Module { get; private set; }

		public ModuleDisconnectedMessage(string module)
		{
			Module = module;
		}
	}
}
