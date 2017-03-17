using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace VASSALChatter
{
	public partial class MenuPage : ContentPage
	{
		public MenuPage(RootPage root)
		{
			InitializeComponent();

			BindingContext = new BaseViewModel(root)
			{
				Title = "VASSAL Chat"
			};


		}
	}
}
