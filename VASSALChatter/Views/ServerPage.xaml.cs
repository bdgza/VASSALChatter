﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace VASSALChatter
{
	public partial class ServerPage : ContentPage
	{
		private ServerViewModel ViewModel
		{
			get { return BindingContext as ServerViewModel; }
		}

		public ServerPage()
		{
			InitializeComponent();

			BindingContext = new ServerViewModel();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			if (ViewModel == null || ViewModel.IsBusy || ViewModel.ModuleItems.Count > 0) return;

			ViewModel.LoadItemsCommand.Execute(null);
		}
	}
}
