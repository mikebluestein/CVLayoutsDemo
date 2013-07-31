using System;
using System.Drawing;
using MonoTouch.Dialog.Utilities;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace CVLayoutsDemo
{
	public class BaseballPlayerCell : UICollectionViewCell
	{
		UIView backContentView;
		UITextView bioTextView;

		string playerBio;

		public string PlayerBio {
			get {
				return playerBio;
			}
			set {
				playerBio = value;
				bioTextView.Text = playerBio;
				bioTextView.SetNeedsDisplay ();
			}
		}

		public UIImageView ImageView { get; private set; }
		
		[Export ("initWithFrame:")]
		public BaseballPlayerCell (RectangleF frame) : base (frame)
		{
			float cornerRadius = 40.0f;

			ImageView = new UIImageView (new RectangleF (10, 10, 2 * cornerRadius, 2 *  cornerRadius)); 
			ImageView.BackgroundColor = UIColor.Clear;
			ImageView.ContentMode = UIViewContentMode.ScaleAspectFill;
			ImageView.Layer.CornerRadius = cornerRadius;
			ImageView.Layer.MasksToBounds = true;
			ImageView.Layer.BorderWidth = 2;
			ImageView.Layer.BorderColor = UIColor.Gray.CGColor;

			ContentView.AddSubview (ImageView);

			backContentView = new UIView (new RectangleF (0, 0, 100, 100)){
				BackgroundColor = UIColor.Gray
			};
			backContentView.Layer.CornerRadius = cornerRadius - 20;

			bioTextView = new UITextView (new RectangleF (backContentView.Frame.X + 10, backContentView.Frame.Y + 10, 
			                                              backContentView.Frame.Width - 20, backContentView.Frame.Height - 20)){
				BackgroundColor = UIColor.Gray,
				UserInteractionEnabled = false
			};

			backContentView.AddSubview (bioTextView);

			AddGestureRecognizer (new UITapGestureRecognizer (() => {
			
				UIView.Transition (
				    fromView: ImageView,
					toView: backContentView,
					duration: 0.5f,
					options: UIViewAnimationOptions.TransitionFlipFromTop | UIViewAnimationOptions.CurveEaseInOut,
					completion: () => { });
			}));

			backContentView.AddGestureRecognizer (new UITapGestureRecognizer (() => { 
	
				UIView.Transition (
					fromView: backContentView,
					toView: ImageView,
					duration: 0.5f,
					options: UIViewAnimationOptions.TransitionFlipFromBottom | UIViewAnimationOptions.CurveEaseInOut,
					completion: () => { });
			}));
		}

		public void UpdateImage (string path)
		{
			using (var image = UIImage.FromFile(path)) {
				ImageView.Image = image;
			}
		}
	}
}