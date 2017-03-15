using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace VASSALChatter
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			FormsMessenger.Instance.Subscribe<DataReceivedMessage>(this, (obj, args) =>
			{
				Debug.WriteLine($"DATA RECEIVED\n\t{args.Module}\n{args.Message}");
			});

			FormsMessenger.Instance.Subscribe<ModuleConnectedMessage>(this, (obj, args) =>
			{
				Debug.WriteLine($"MODULE CONNECTED\n\t{args.Connection.Module}");
			});

			FormsMessenger.Instance.Subscribe<ModuleDisconnectedMessage>(this, (obj, args) =>
			{
				Debug.WriteLine($"MODULE DISCONNECTED\n\t{args.Module}");
			});

			//DependencyService.Get<IVassalSocketManager>().OpenModuleConnection("Memoir '44");

		}
	}
}
