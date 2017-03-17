using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace VASSALChatter
{
	public partial class RoomsPage : ContentPage
	{
		private ChatViewModel ViewModel
		{
			get { return BindingContext as ChatViewModel; }
		}

		public RoomsPage(ChatViewModel viewModel)
		{
			InitializeComponent();

			BindingContext = viewModel;
		}
	}
}
