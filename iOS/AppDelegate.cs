using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Foundation;
using UIKit;

namespace VASSALChatter.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		private NSObject keyboardShowObserver;
		private NSObject keyboardHideObserver;

		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init();

			LoadApplication(new App());

			SetupKeyboardListeners();

			return base.FinishedLaunching(app, options);
		}

		private void SetupKeyboardListeners()
		{
			keyboardShowObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, (notification) =>
			{
				NSValue nsKeyboardBounds = (NSValue)notification.UserInfo.ObjectForKey(UIKeyboard.BoundsUserInfoKey);
				RectangleF keyboardBounds = nsKeyboardBounds.RectangleFValue;

				FormsMessenger.Instance.Send<KeyboardVisibilityMessage>(new KeyboardVisibilityMessage { Visible = true, Height = keyboardBounds.Height }, this);
			});

			keyboardHideObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, (notification) =>
			{
				FormsMessenger.Instance.Send<KeyboardVisibilityMessage>(new KeyboardVisibilityMessage { Visible = false }, this);
			});
		}
}
}
