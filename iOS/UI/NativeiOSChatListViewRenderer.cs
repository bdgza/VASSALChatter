using UIKit;
using VASSALChatter;
using VASSALChatter.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NativeChatListView),
						  typeof(NativeiOSChatListViewRenderer))]

namespace VASSALChatter.iOS
{
	public class NativeiOSChatListViewRenderer : ListViewRenderer
	{
		private ChatInputAccessoryView _chatInputAccessoryView;

		public override UIView InputAccessoryView
		{
			get { return _chatInputAccessoryView; }
		}

		public override bool CanBecomeFirstResponder
		{
			get { return true; }
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null)
			{
				// Unsubscribe
			}

			if (e.NewElement != null)
			{
				var element = e.NewElement as NativeChatListView;
				_chatInputAccessoryView = new ChatInputAccessoryView("", element.SendMessage);
				_chatInputAccessoryView.WillAppear();
				this.BecomeFirstResponder();
			}
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
		}

		public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			return new SizeRequest(new Size(Frame.Width, 200));
		}
	}
}
