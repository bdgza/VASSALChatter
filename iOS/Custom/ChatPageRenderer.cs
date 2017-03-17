using AVFoundation;
using CoreGraphics;
using Foundation;
using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using VASSALChatter.iOS;
using VASSALChatter;

[assembly: ExportRenderer(typeof(ChatPage), typeof(ChatPageRenderer))]
namespace VASSALChatter.iOS
{
	public class ChatPageRenderer : PageRenderer
	{
		UIView liveCameraStream;
		UIButton takePhotoButton;
		UIButton toggleCameraButton;
		UIButton toggleFlashButton;

		protected override void OnElementChanged(VisualElementChangedEventArgs e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null || Element == null)
			{
				return;
			}

			System.Diagnostics.Debug.WriteLine("Got ChatPageRenderer OnElementChanged");

			try
			{
				SetupUserInterface();
				SetupEventHandlers();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(@"			ERROR: ", ex.Message);
			}
		}

		void SetupUserInterface()
		{
			//var centerButtonX = View.Bounds.GetMidX() - 35f;
			//var topLeftX = View.Bounds.X + 25;
			//var topRightX = View.Bounds.Right - 65;
			//var bottomButtonY = View.Bounds.Bottom - 150;
			//var topButtonY = View.Bounds.Top + 15;
			//var buttonWidth = 70;
			//var buttonHeight = 70;

			//liveCameraStream = new UIView()
			//{
			//	Frame = new CGRect(0f, 0f, View.Bounds.Width, View.Bounds.Height)
			//};

			//takePhotoButton = new UIButton()
			//{
			//	Frame = new CGRect(centerButtonX, bottomButtonY, buttonWidth, buttonHeight)
			//};
			//takePhotoButton.SetBackgroundImage(UIImage.FromFile("chat.png"), UIControlState.Normal);

			//toggleCameraButton = new UIButton()
			//{
			//	Frame = new CGRect(topRightX, topButtonY + 5, 35, 26)
			//};
			//toggleCameraButton.SetBackgroundImage(UIImage.FromFile("connect.png"), UIControlState.Normal);

			//toggleFlashButton = new UIButton()
			//{
			//	Frame = new CGRect(topLeftX, topButtonY, 37, 37)
			//};
			//toggleFlashButton.SetBackgroundImage(UIImage.FromFile("disconnect.png"), UIControlState.Normal);

			//View.Add(liveCameraStream);
			//View.Add(takePhotoButton);
			//View.Add(toggleCameraButton);
			//View.Add(toggleFlashButton);
		}

		void SetupEventHandlers()
		{
			//takePhotoButton.TouchUpInside += (object sender, EventArgs e) =>
			//{
			//	CapturePhoto();
			//};

			//toggleCameraButton.TouchUpInside += (object sender, EventArgs e) =>
			//{
			//	ToggleFrontBackCamera();
			//};

			//toggleFlashButton.TouchUpInside += (object sender, EventArgs e) =>
			//{
			//	ToggleFlash();
			//};
		}
	}
}


