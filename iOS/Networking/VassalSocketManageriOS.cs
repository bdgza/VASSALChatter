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
		private readonly long epoch = (long)((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds);

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

			IAsyncResult asyncConnect = clientSocket.BeginConnect(host.GetIPEndPoint(), new AsyncCallback(OpenModuleConnectionCallback), moduleSocket);
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
				IAsyncResult asyncSend = clientSocket.BeginSend(msg, 0, msg.Length, SocketFlags.None, new AsyncCallback(ModuleSocketSendCallback), moduleSocket);

				IAsyncResult asyncReceive = clientSocket.BeginReceive(moduleSocket.sBuffer, 0, moduleSocket.sBuffer.Length, SocketFlags.None, new AsyncCallback(ModuleSocketReceiveCallback), moduleSocket);
			}
		}

		private void ModuleSocketSendCallback(IAsyncResult asyncResult)
		{
			ModuleSocket moduleSocket = (ModuleSocket)asyncResult.AsyncState;
			Socket clientSocket = moduleSocket.Socket;
			int bytesSent = clientSocket.EndSend(asyncResult);
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

		public void Test()
		{
			IPAddress ipAddress = Dns.GetHostAddresses("one.vassalengine.org")[0];

			IPEndPoint ipEndpoint = new IPEndPoint(ipAddress, 5050);

			Console.WriteLine($"Connection to {ipAddress.GetAddressBytes()[0]}.{ipAddress.GetAddressBytes()[1]}.{ipAddress.GetAddressBytes()[2]}.{ipAddress.GetAddressBytes()[3]}:5050");

			Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			IAsyncResult asyncConnect = clientSocket.BeginConnect(ipEndpoint, new AsyncCallback(connectCallback), clientSocket);

			Console.Write("Connection in progress.");
		}

		public void connectCallback(IAsyncResult asyncConnect)
		{
			Socket clientSocket = (Socket)asyncConnect.AsyncState;
			clientSocket.EndConnect(asyncConnect);
			// arriving here means the operation completed
			// (asyncConnect.IsCompleted = true) but not
			// necessarily successfully
			if (clientSocket.Connected == false)
			{
				Console.WriteLine(".client is not connected.");
				return;
			}
			else Console.WriteLine(".client is connected.");

			var strMsg = $"REG\tchatter8.{epoch}\tMemoir '44/Main Room\tname=chatter|id=chatter8.{epoch}|moduleVersion=10.12|looking=false|profile=|client=3.2.17|away=true|crc=1b732b83\n";

			Console.WriteLine($"MSG = {strMsg}");

			byte[] sendBuffer = Encoding.ASCII.GetBytes(strMsg);
			IAsyncResult asyncSend = clientSocket.BeginSend(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, new AsyncCallback(sendCallback), clientSocket);

			Console.Write("Sending data.");
		}

		public void sendCallback(IAsyncResult asyncSend)
		{
			Socket clientSocket = (Socket)asyncSend.AsyncState;
			int bytesSent = clientSocket.EndSend(asyncSend);
			Console.WriteLine(".{0} bytes sent.", bytesSent.ToString());

			StateObject stateObject = new StateObject(2048, clientSocket);

			// this call passes the StateObject because it
			// needs to pass the buffer as well as the socket
			IAsyncResult asyncReceive = clientSocket.BeginReceive(stateObject.sBuffer, 0, stateObject.sBuffer.Length, SocketFlags.None, new AsyncCallback(receiveCallback), stateObject);

			Console.Write("Receiving response...");
		}

		public void receiveCallback(IAsyncResult asyncReceive)
		{
			try
			{
				StateObject stateObject = (StateObject)asyncReceive.AsyncState;
				Socket clientSocket = (Socket)stateObject.sSocket;

				int bytesReceived = stateObject.sSocket.EndReceive(asyncReceive);

				if (bytesReceived == 0)
				{
					// server has disconnected us
					FormsMessenger.Instance.Send(new ModuleDisconnectedMessage("Memoir '44"));

					Console.WriteLine("Shutting down.");

					stateObject.sSocket.Shutdown(SocketShutdown.Both);
					stateObject.sSocket.Close();

					return;
				}

				var received = Encoding.ASCII.GetString(stateObject.sBuffer);

				Console.WriteLine(".{0} bytes received: {1}{2}{2}", bytesReceived.ToString(), received, Environment.NewLine);

				FormsMessenger.Instance.Send(new DataReceivedMessage("Memoir '44", received));

				if (received.StartsWith("PRIV_CHAT", StringComparison.Ordinal))
				{
					Console.WriteLine("Sending shutdown.");

					clientSocket.Send(Encoding.ASCII.GetBytes("!BYE\n"));

					stateObject.sSocket.Shutdown(SocketShutdown.Both);
					stateObject.sSocket.Close();
				}
				else
				{
					asyncReceive = clientSocket.BeginReceive(stateObject.sBuffer, 0, stateObject.sBuffer.Length, SocketFlags.None, new AsyncCallback(receiveCallback), stateObject);

					Console.Write("Receiving response...");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"EXCEPTION IN RECEIVE {ex.GetType()} {ex.Message}");

				FormsMessenger.Instance.Send(new ModuleDisconnectedMessage("Memoir '44"));
			}
		}

	}
}
