using System;
namespace VASSALChatter
{
	public class DataReceivedMessage : IMessage
	{
		public string Module { get; private set; }
		public string Message { get; private set; }
		
		public DataReceivedMessage(string module, string message)
		{
			Module = module;
			Message = message;
		}
	}
}
