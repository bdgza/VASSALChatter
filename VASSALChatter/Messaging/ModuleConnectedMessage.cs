using System;
namespace VASSALChatter
{
	public class ModuleConnectedMessage : IMessage
	{
		public VassalConnection Connection { get; private set; }
		
		public ModuleConnectedMessage(VassalConnection connection)
		{
			Connection = connection;
		}
	}
}
