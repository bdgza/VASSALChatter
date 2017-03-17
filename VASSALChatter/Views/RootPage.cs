using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace VASSALChatter
{
	public class RootPage : MasterDetailPage
	{
		List<VassalConnection> VassalConnections { get; set; } = new List<VassalConnection>();
		List<ChatTabsPage> ChatPages { get; set; } = new List<ChatTabsPage>();

		public RootPage()
		{
			//Pages = new Dictionary<MenuType, NavigationPage>();
			BindingContext = new BaseViewModel(this)
			{
				Title = "VASSAL Chat",
				Icon = "hamburger.png"
			};

			SetupListeners();

			Master = new MenuPage(this);

			//setup home page
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			NavigateAsync(new PageDefinition() { PageType = PageType.Chat, Id = "Chat Module" });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

			InvalidateMeasure();
		}

		private void SetupListeners()
		{
			FormsMessenger.Instance.Subscribe<ModuleConnectedMessage>(this, (obj, args) =>
			{
				VassalConnections.Add(args.Connection);
			});

			FormsMessenger.Instance.Subscribe<ModuleDisconnectedMessage>(this, (obj, args) =>
			{
				VassalConnections.RemoveAll((con) => con.Module == args.Module);
			});
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
					newPage = new BaseNavigationPage(new ServerPage(this));
					break;
				case PageType.Chat:
					newPage = new BaseNavigationPage(new ChatTabsPage(this, page.Id));
					break;
			}
			if (newPage == null) return;

			Detail = newPage;
		}

		public void ConnectToModule(string moduleName)
		{
			DependencyService.Get<IVassalSocketManager>().OpenModuleConnection(moduleName);
		}

		public void DisconnectFromModule(string moduleName)
		{
			DependencyService.Get<IVassalSocketManager>().CloseModuleConnection(moduleName);
		}

		internal void SendChatMessage(string moduleName, string room, string msg)
		{
			DependencyService.Get<IVassalSocketManager>().SendChatMessage(moduleName, room, msg);
		}
	}
}

