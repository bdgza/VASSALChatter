using System;
using Xamarin.Forms;

namespace VASSALChatter
{
	public class BaseNavigationPage : NavigationPage
	{
		public BaseNavigationPage(Page root) : base(root)
        {
			Init();
		}

		public BaseNavigationPage()
		{
			Init();
		}

		void Init()
		{
			Title = "VASSAL Chat";
			BarBackgroundColor = (Color) Application.Current.Resources["BarColor"];
			BarTextColor = (Color)Application.Current.Resources["BarTextColor"];
		}
	}
}
