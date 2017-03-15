using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace VASSALChatter
{
	public partial class MenuPage : ContentPage
	{
		private RootPage root;

		public MenuPage(RootPage root)
		{
			this.root = root;

			InitializeComponent();

			BindingContext = new BaseViewModel
			{
				Title = "VASSAL Chat"
			};


		}
	}
}
