using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;

namespace VASSALChatter
{
	public partial class ChatPage : ContentPage
	{
		private ChatViewModel ViewModel
		{
			get { return BindingContext as ChatViewModel; }
		}

		public ChatPage()
		{
			BindingContext = new ChatViewModel(new RootPage(), "Chat Module");

			InitializeComponent();

			SetupListeners();
		}

		public ChatPage(ChatViewModel viewModel)
		{
			BindingContext = viewModel;

			InitializeComponent();

			SetupListeners();
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			FormsMessenger.Instance.Unsubscribe<DataReceivedMessage>(this);
			FormsMessenger.Instance.Unsubscribe<KeyboardVisibilityMessage>(this);
		}

		private void SetupListeners()
		{
			FormsMessenger.Instance.Subscribe<DataReceivedMessage>(this, (obj, args) =>
			{
				if (args.Module == ViewModel.ModuleName)
				{
					if (args.Message.Length >= 4 && args.Message.Substring(0, 4) == "CHAT")
					{
						// CHAT message
						AddMessage(new ChatMessage { Value = args.Message.Substring(4, args.Message.Length - 5), Color = Color.Black });
					}
				}
			});

			FormsMessenger.Instance.Subscribe<KeyboardVisibilityMessage>(this, (obj, args) =>
			{
				Debug.WriteLine($"KEYBOARD VISIBLE = {args.Visible}");

				var chatMsg = ViewModel.Messages.LastOrDefault();
				if (chatMsg != null)
					Device.BeginInvokeOnMainThread(() => listView.ScrollTo(chatMsg, ScrollToPosition.MakeVisible, true));
			});
		}

		private void AddMessage(ChatMessage chatMsg)
		{
			ViewModel.Messages.Add(chatMsg);
			Device.BeginInvokeOnMainThread(() => listView.ScrollTo(chatMsg, ScrollToPosition.MakeVisible, true));
		}

		private void SendButton_Clicked(object sender, System.EventArgs e)
		{
			SendMessage();
		}

		void ChatEntry_Completed(object sender, System.EventArgs e)
		{
			SendMessage();
		}

		private void SendMessage()
		{
			// TODO: username
			var msg = ChatMessage.FormatChatMessage("chatter", ViewModel.MessageField);
			ViewModel.MessageField = "";
			// TODO: current room name
			ViewModel.Root.SendChatMessage(ViewModel.ModuleName, "Main Room", msg);
			AddMessage(new ChatMessage { Value = msg, Color = Color.Gray });
		}

		void ListView_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
		{
			MessageEntry.Unfocus();
			if (e == null) return;
			listView.SelectedItem = null;
		}

		void ListView_Focused(object sender, Xamarin.Forms.FocusEventArgs e)
		{
			Debug.WriteLine("ListView_Focused");
			MessageEntry.Unfocus();
		}

		void Grid_Focused(object sender, Xamarin.Forms.FocusEventArgs e)
		{
			Debug.WriteLine("ListView_Focused");
			MessageEntry.Unfocus();
		}
	}
}
