using System;
namespace VASSALChatter
{
	public enum PageType
	{
		Server,
		Module,
		Person,
		Chat
	}

	public class PageDefinition
	{
		public PageDefinition()
		{
			PageType = PageType.Server;
		}

		public PageType PageType { get; set; }
		public string Id { get; set; }
	}
}
