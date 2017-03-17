using System;
namespace VASSALChatter
{
	public class KeyboardVisibilityMessage : IMessage
	{
		public bool Visible { get; set; }
		public float Height { get; set; }
	}
}
