using System;
using System.Threading.Tasks;

namespace VASSALChatter
{
	public interface IVassalSocketManager
	{
		void OpenModuleConnection(string module);
		void CloseModuleConnection(string module);
		void SendChatMessage(string module, string room, string message);
	}
}
