using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using VASSALChatter.iOS.Networking;
using System.Collections.Generic;

[assembly: Xamarin.Forms.Dependency(typeof(VassalSocketManageriOS))]

namespace VASSALChatter.iOS.Networking
{
	public class VassalSocketManageriOS : IVassalSocketManager
	{
		public VassalSocketManageriOS()
		{
		}

		private IList<ModuleSocket> openModuleSockets = new List<ModuleSocket>();

		class StateObject
		{
			internal byte[] sBuffer;
			internal Socket sSocket;

			internal StateObject(int size, Socket sock)
			{
				sBuffer = new byte[size];
				sSocket = sock;
			}
		}

		public void OpenModuleConnection(string module)
		{
			var host = NodeAddress.RequestHostAddress();

			Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			ModuleSocket moduleSocket = new ModuleSocket(16384, clientSocket, module);

			clientSocket.BeginConnect(host.GetIPEndPoint(), new AsyncCallback(OpenModuleConnectionCallback), moduleSocket);
		}

		private void OpenModuleConnectionCallback(IAsyncResult asyncConnect)
		{
			ModuleSocket moduleSocket = (ModuleSocket)asyncConnect.AsyncState;
			Socket clientSocket = moduleSocket.Socket;
			clientSocket.EndConnect(asyncConnect);

			if (clientSocket.Connected)
			{
				// message that we are connected

				openModuleSockets.Add(moduleSocket);

				FormsMessenger.Instance.Send(new ModuleConnectedMessage(
					new VassalConnection
					{
						Module = moduleSocket.Module
					}));

				// join the chat room

				var msg = Encoding.ASCII.GetBytes($"REG\t{moduleSocket.Password}.{moduleSocket.Timestamp}\t{moduleSocket.Module}/Main Room\tname=chatter|id={moduleSocket.Password}.{moduleSocket.Timestamp}|moduleVersion=10.12|looking=false|profile=|client=3.2.17|away=true|crc=1b732b83\n");
				clientSocket.BeginSend(msg, 0, msg.Length, SocketFlags.None, new AsyncCallback(ModuleSocketSendCallback), moduleSocket);

				clientSocket.BeginReceive(moduleSocket.sBuffer, 0, moduleSocket.sBuffer.Length, SocketFlags.None, new AsyncCallback(ModuleSocketReceiveCallback), moduleSocket);
			}
		}

		private void ModuleSocketSendCallback(IAsyncResult asyncResult)
		{
			ModuleSocket moduleSocket = (ModuleSocket)asyncResult.AsyncState;
			Socket clientSocket = moduleSocket.Socket;
			int bytesSent = clientSocket.EndSend(asyncResult);

			Console.WriteLine($"Sent {bytesSent} bytes to server");
		}

		private void RemoveModuleSocket(ModuleSocket moduleSocket)
		{
			FormsMessenger.Instance.Send(new ModuleDisconnectedMessage(moduleSocket.Module));

			moduleSocket.Socket?.Shutdown(SocketShutdown.Both);
			moduleSocket.Socket?.Close();

			openModuleSockets.Remove(moduleSocket);
		}

		private void ModuleSocketReceiveCallback(IAsyncResult asyncResult)
		{
			ModuleSocket moduleSocket = (ModuleSocket)asyncResult.AsyncState;

			try
			{
				Socket clientSocket = moduleSocket.Socket;

				int bytesReceived = moduleSocket.Socket.EndReceive(asyncResult);

				Console.WriteLine($"Received {bytesReceived} bytes from server");

				if (bytesReceived == 0)
				{
					// server has disconnected us
					RemoveModuleSocket(moduleSocket);

					return;
				}

				var received = Encoding.ASCII.GetString(moduleSocket.sBuffer);

				Console.WriteLine(".{0} bytes received: {1}{2}{2}", bytesReceived.ToString(), received, Environment.NewLine);

				FormsMessenger.Instance.Send(new DataReceivedMessage(moduleSocket.Module, received));

				if (received.StartsWith("PRIV_CHAT", StringComparison.Ordinal))
				{
					Console.WriteLine("Sending shutdown.");

					clientSocket.Send(Encoding.ASCII.GetBytes("!BYE\n"));

					RemoveModuleSocket(moduleSocket);
				}
				else
				{
					asyncResult = clientSocket.BeginReceive(moduleSocket.sBuffer, 0, moduleSocket.sBuffer.Length, SocketFlags.None, new AsyncCallback(ModuleSocketReceiveCallback), moduleSocket);

					Console.Write("Receiving response...");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"EXCEPTION IN RECEIVE {ex.GetType()} {ex.Message}");

				RemoveModuleSocket(moduleSocket);
			}
		}

	}
}
