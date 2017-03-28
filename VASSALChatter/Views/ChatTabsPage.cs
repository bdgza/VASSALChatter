using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace VASSALChatter
{
	public class ChatTabsPage : TabbedPage
	{
		private ChatViewModel ViewModel
		{
			get { return BindingContext as ChatViewModel; }
		}

		public ChatTabsPage(RootPage root, string moduleName)
		{
			BindingContext = new ChatViewModel(root, moduleName);

			Title = moduleName;

			Children.Add(new RoomsPage(ViewModel));
			Children.Add(new ChatPage(ViewModel));

			CurrentPage = Children[1];

			SetupListeners();

			ChangeToolbarItems();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			DelayedConnect();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
		}

		private async Task DelayedConnect()
		{
			await DelayActionAsync(300, () =>
			{
				ViewModel.Root.ConnectToModule(ViewModel.ModuleName);
			});
		}

		public async Task DelayActionAsync(int delay, Action action)
		{
			await Task.Delay(delay);

			action();
		}

		protected override void OnDisappearing()
		{
			FormsMessenger.Instance.Unsubscribe<DataReceivedMessage>(this);
			FormsMessenger.Instance.Unsubscribe<ModuleConnectedMessage>(this);
			FormsMessenger.Instance.Unsubscribe<ModuleDisconnectedMessage>(this);
		}

		private void SetupListeners()
		{
			FormsMessenger.Instance.Subscribe<DataReceivedMessage>(this, async (obj, args) =>
			{
				if (args.Module == ViewModel.ModuleName)
				{
					await ProcessMessage(args.Message);
				}
			});

			FormsMessenger.Instance.Subscribe<ModuleConnectedMessage>(this, (obj, args) =>
			{
				ViewModel.IsConnected = true;

				ChangeToolbarItems();
			});

			FormsMessenger.Instance.Subscribe<ModuleDisconnectedMessage>(this, (obj, args) =>
			{
				ViewModel.IsConnected = false;

				ChangeToolbarItems();
			});
		}

		private void ChangeToolbarItems()
		{
			Device.BeginInvokeOnMainThread(() =>
			{
				ToolbarItems.Clear();

				if (ViewModel.IsConnected)
				{
					ToolbarItems.Add(new ToolbarItem("Disconnect", "disconnect.png", () =>
					{
						ViewModel.Root.DisconnectFromModule(ViewModel.ModuleName);
					}));
				}
				else
				{
					ToolbarItems.Add(new ToolbarItem("Connect", "connect.png", () =>
					{
						ViewModel.Root.ConnectToModule(ViewModel.ModuleName);
					}));
				}
			});
		}

		private async Task ProcessMessage(string message)
		{
			await Task.Run(() =>
			{
				if (message.Length >= 4 && message.Substring(0, 4) == "CHAT")
				{
					// CHAT message
					//var chat = message.Substring(4);
					//ViewModel.Messages.Add(new ChatMessage { Value = chat });
				}
				else if (message.Length >= 4 && message.Substring(0, 4) == "SYNC")
				{
					// SYNC request, ignore
				}
				else if (message.Length >= 15 && message.Substring(0, 15) == "SERVER_CONTENTS")
				{
					// SERVER_CONTENTS
					// TODO: ?
				}
				else if (message.Length >= 9 && message.Substring(0, 9) == "PRIV_CHAT")
				{
					// PRIV_CHAT private chat message
					// TODO
				}
				else
				{
					var fields = message.Split('\t');

					switch (fields[0])
					{
						case "FWD":
							// CHAT message
							// TODO: got a FWD
							Debug.WriteLine("GOT A FWD!");
							break;
						case "LIST":
							// LIST the users and the rooms
							for (var f = 1; f < fields.Length; f++)
							{
								var field = fields[f];
								var index = field.IndexOf('=');
								var header = field.Length > 5 ? field.Substring(5, index - 5) : "";
								var headerFields = header.Split('/');
								var roomName = headerFields[1];
								var userId = headerFields[2];
								var userInfo = field.Substring(index + 1);

								Debug.WriteLine($"LIST: {roomName}: {userId}");
							}
							break;
						case "ROOM_INFO":
							// TODO: room info
							break;
						case "PLAY":
							// TODO: play a sound, like wake up
							break;
						default:
							Debug.WriteLine($"UNKNOWN CHAT MESSAGE: {fields[0]} for {message}");
							break;
					}
				}
			});
		}
	}
}

