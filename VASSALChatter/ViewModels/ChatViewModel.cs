using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VASSALChatter
{
	public class ChatViewModel : BaseViewModel
	{
		public string ModuleName { get; private set; }
		public bool IsConnected { get; set; }

		public ObservableCollection<ChatMessage> Messages { get; private set; } = new ObservableCollection<ChatMessage>();

		private string messageField = string.Empty;
		public const string MessageFieldPropertyName = "MessageField";

		/// <summary>
		/// Gets or sets the "Title" property
		/// </summary>
		/// <value>The title.</value>
		public string MessageField
		{
			get { return messageField; }
			set { SetProperty(ref messageField, value); }
		}

		public ChatViewModel(RootPage root, string moduleName)
			: base(root)
		{
			Title = "Chat";
			Icon = "hamburger.png";
			ModuleName = moduleName;
		}
	}
}

