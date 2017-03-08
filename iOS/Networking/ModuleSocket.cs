using System;
using System.Linq;
using System.Net.Sockets;

namespace VASSALChatter.iOS
{
	public class ModuleSocket
	{
		private static Random random = new Random();

		internal byte[] sBuffer;
		internal Socket Socket { get; private set; }
		internal string Module { get; private set; }
		internal string Password { get; private set; }
		internal readonly long Timestamp = (long)((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds);

		private static string RandomString(int length)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			return new string(Enumerable.Repeat(chars, length)
			  .Select(s => s[random.Next(s.Length)]).ToArray());
		}

		internal ModuleSocket(int size, Socket sock, string module)
		{
			sBuffer = new byte[size];
			Socket = sock;
			Module = module;
			Password = RandomString(64);
		}
	}
}
