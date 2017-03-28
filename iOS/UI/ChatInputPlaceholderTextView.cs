using System;
using UIKit;

namespace VASSALChatter.iOS
{
	public partial class ChatInputPlaceholderTextView : UITextField
	{
		public ChatInputPlaceholderTextView(IntPtr handle)
			: base(handle)
		{
			Placeholder = string.Empty;
			ReturnKeyType = UIReturnKeyType.Send;
		}

		public ChatInputPlaceholderTextView()
			: base()
		{
			Placeholder = string.Empty;
			ReturnKeyType = UIReturnKeyType.Send;
		}

		public string Placeholder { get; set; }

		private UILabel PlaceholderLabel { get; set; }


		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			if (PlaceholderLabel == null)
			{
				PlaceholderLabel = new UILabel();
				PlaceholderLabel.TranslatesAutoresizingMaskIntoConstraints = false;
				PlaceholderLabel.TextColor = UIColor.LightGray;
				PlaceholderLabel.Font = Font;
				PlaceholderLabel.Text = Placeholder;

				AddSubview(PlaceholderLabel);

				AddConstraint(NSLayoutConstraint.Create(PlaceholderLabel, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, this, NSLayoutAttribute.CenterY, 1, 0));
				AddConstraint(NSLayoutConstraint.Create(PlaceholderLabel, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, this, NSLayoutAttribute.Leading, 1, 10));
				AddConstraint(NSLayoutConstraint.Create(PlaceholderLabel, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, this, NSLayoutAttribute.Trailing, 1, 0));

				PlaceholderLabel.Hidden = !string.IsNullOrEmpty(Text);
			}
		}

		public void Reset()
		{
			Text = string.Empty;
			UpdatePlaceholder();
		}

		private void UpdatePlaceholder()
		{
			if (PlaceholderLabel != null)
			{
				PlaceholderLabel.Hidden = !string.IsNullOrEmpty(Text);
			}
		}


	}
}
