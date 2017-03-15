using System;
namespace VASSALChatter
{
	public enum PageType
	{
		Server,
		Module,
		Person
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
