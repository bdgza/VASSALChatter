using System;
using Xamarin.Forms;

namespace VASSALChatter
{
	public class ChatMessage
	{
		public string Value { get; set; }
		public Color Color { get; set; } = Color.Gray;

		public static string FormatChatMessage(string userName, string message)
		{
			return $"[{userName}] - {message}";
		}
	}
}
