using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace VASSALChatter
{
	public class RootPage : MasterDetailPage
	{
		//Dictionary<MenuType, NavigationPage> Pages { get; set; }

		public RootPage()
		{
			//Pages = new Dictionary<MenuType, NavigationPage>();
			BindingContext = new BaseViewModel
			{
				Title = "VASSAL Chat",
				Icon = "hamburger.png"
			};
			Master = new MenuPage(this);

			//setup home page
			NavigateAsync(new PageDefinition());

			InvalidateMeasure();
		}

		public async Task NavigateAsync(PageDefinition page)
		{

			if (Detail != null)
			{
				if (Device.OS == TargetPlatform.Android)
					await Task.Delay(300);
			}

			Page newPage = null;
			switch (page.PageType)
			{
				case PageType.Server:
					newPage = new BaseNavigationPage(new ServerPage());
					break;
			}
			if (newPage == null) return;

			Detail = newPage;
		}
	}
}

