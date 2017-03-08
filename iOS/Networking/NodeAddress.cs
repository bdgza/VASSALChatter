using System;
using System.Net;

namespace VASSALChatter.iOS
{
	public class NodeAddress
	{
		public string Host { get; set; }
		public IPAddress IP { get; set; }
		public int Port { get; set; }

		public IPEndPoint GetIPEndPoint()
		{
			return new IPEndPoint(IP, Port);
		}

		public static NodeAddress RequestHostAddress()
		{
			var host = new NodeAddress { Host = "www.vassalengine.org", Port = 5050, IP = new IPAddress(new byte[] { 62, 210, 178, 7 }) };
			WebClient client = new WebClient();
			string nodeString = client.DownloadString("http://www.vassalengine.org/util/getServerImpl");

			string[] nodeLines = nodeString.Split((char)0x0A);

			foreach (string line in nodeLines)
			{
				string[] fields = line.Split(new string[] { " = " }, StringSplitOptions.None);

				switch (fields[0])
				{
					case "nodeHost":
						host.Host = fields[1];
						break;
					case "nodePost":
						host.Port = int.Parse(fields[1]);
						break;
				}
			}

			IPAddress[] ipAddresses = Dns.GetHostAddresses("one.vassalengine.org");
			if (ipAddresses.Length > 0)
				host.IP = ipAddresses[0];

			return host;
		}
	}
}
