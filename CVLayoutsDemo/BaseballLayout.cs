using System;
using System.Drawing;
using MonoTouch.CoreAnimation;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

namespace CVLayoutsDemo
{
	public class BaseballLayout : UICollectionViewLayout
	{
		static NSString baseFieldDecorationId = new NSString ("BaseballFieldDecoration");

		int count;
		PointF center;
		SizeF size;

		public SizeF ItemSize { get; set; }

		public bool IsHomeTeamOnField { get; set; }
		
		public BaseballLayout ()
		{
			RegisterClassForDecorationView (typeof(BaseballFieldDecoration), baseFieldDecorationId);

			IsHomeTeamOnField = true;
		}

		public override void PrepareLayout ()
		{
			base.PrepareLayout ();
			
			size = CollectionView.Frame.Size;
			count = CollectionView.NumberOfItemsInSection (0);
			center = new PointF (size.Width / 2.0f, size.Height / 2.0f);	
		}

		public override SizeF CollectionViewContentSize {
			get {
				return size;
			}
		}

		public override bool ShouldInvalidateLayoutForBoundsChange (RectangleF newBounds)
		{
			return true;
		}

		public override UICollectionViewLayoutAttributes LayoutAttributesForDecorationView (NSString kind, NSIndexPath indexPath)
		{
			var decorationAttribs = UICollectionViewLayoutAttributes.CreateForDecorationView (kind, indexPath);
			decorationAttribs.Size = UIScreen.MainScreen.Bounds.Size;
			decorationAttribs.Center = CollectionView.Center;
			decorationAttribs.ZIndex = -1;
 		
			return decorationAttribs;
		}

		public override UICollectionViewLayoutAttributes LayoutAttributesForItem (NSIndexPath path)
		{
			UICollectionViewLayoutAttributes attribs = null;

			if (path.Section == 0) {
				attribs = LayoutHomeTeam (path);
			} else {
				attribs = LayoutAwayTeam (path);
			}

			return attribs;
		}

		UICollectionViewLayoutAttributes LayoutHomeTeam (NSIndexPath indexPath)
		{
			var attributes = UICollectionViewLayoutAttributes.CreateForCell (indexPath);
			attributes.Size = ItemSize;

			double theta = 0;
			float r = 320;
			float yc = 0;
			float x = 0;
			float y = 0;

			if (IsHomeTeamOnField) {

                LayoutFielder(indexPath, ref theta, ref r, ref yc);

				if (indexPath.Row > 8) {
					// home dugout
					/* x = (row - 8) * spacing + offset_along_line */
					x = (indexPath.Row - 8) * 40 + 450;
					y = -x + 1150;

					// make players in the dugout smaller
					attributes.Transform3D = CATransform3D.MakeScale (0.5f, 0.5f, 1.0f);
				} else {
					// fielders
					x = center.X + r * (float)Math.Cos (theta);
					y = center.Y + yc + r * (float)Math.Sin (theta);
				}

				attributes.Center = new PointF (x, y);

			} else {
				// put them all in the home dugout
				x = (indexPath.Row - 8) * 40 + 450;
				y = -x + 1150;
				attributes.Transform3D = CATransform3D.MakeScale (0.5f, 0.5f, 1.0f);
			}

			attributes.Center = new PointF (x, y);

			return attributes;
		}

		UICollectionViewLayoutAttributes LayoutAwayTeam (NSIndexPath indexPath)
		{
			var attributes = UICollectionViewLayoutAttributes.CreateForCell (indexPath);
			attributes.Size = ItemSize;

			double theta = 0;
			float r = 320;
			float yc = 0;
			float x = 0;
			float y = 0;

			if (!IsHomeTeamOnField) {

                LayoutFielder(indexPath, ref theta, ref r, ref yc);

				if (indexPath.Row > 8) {	
					// away dugout
					x = (indexPath.Row - 8)  * 40 + 60;
					y =  (x + 400);

					// make players in the dugout smaller
					attributes.Transform3D = CATransform3D.MakeScale (0.5f, 0.5f, 1.0f);
				} else {
					// fielders
					x = center.X + r * (float)Math.Cos (theta);
					y = center.Y + yc + r * (float)Math.Sin (theta);
				}

				attributes.Center = new PointF (x, y);	

			} else {
				// put them all in the away dugout
				x = (indexPath.Row)  * 40 + 60;
				y =  (x + 400);
				attributes.Transform3D = CATransform3D.MakeScale (0.5f, 0.5f, 1.0f);
			}

			attributes.Center = new PointF (x, y);

			return attributes;
		}

		public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect (RectangleF rect)
		{
			int n = count * 2 + 1;
			var attributes = new UICollectionViewLayoutAttributes [n]; //assume same number of players on each team

			for (int i = 0; i < count; i++) {
				var indexPath = NSIndexPath.FromItemSection (i, 0);
				attributes [i] = LayoutAttributesForItem (indexPath);

				var indexPath2 = NSIndexPath.FromItemSection (i, 1);
				attributes [i+count] = LayoutAttributesForItem (indexPath2);
			}

			// add layout attributes for decoration view
			attributes [n-1] = LayoutAttributesForDecorationView (baseFieldDecorationId, NSIndexPath.FromItemSection (0, 0));

			return attributes;
		}

		static void LayoutFielder(NSIndexPath indexPath, ref double theta, ref float r, ref float yc)
		{
			// outfield
			if (indexPath.Row > 5 && indexPath.Row < 9)
			{
				theta = (9 - indexPath.Row) * Deg(-45);
			}

			// infield
			if (indexPath.Row > 1 && indexPath.Row < 4)
			{
				r = 170;
				theta += (indexPath.Row - 2) * Deg(-45);

			}
			else if (indexPath.Row > 3 && indexPath.Row < 6)
			{
				r = 170;
				theta += (indexPath.Row - 1) * Deg(-45);
			}

			// pitcher and catcher
			if (indexPath.Row == 0 || indexPath.Row == 1)
			{
				r = 0;
				yc = 200 * indexPath.Row;
			}
		}

		static double Deg (double angle)
		{
			return angle * Math.PI / 180;
		}

		class BaseballFieldDecoration : UICollectionReusableView
		{
			UIImageView baseballFieldView;
			UIImage baseballFieldImage;

			[Export ("initWithFrame:")]
			BaseballFieldDecoration (RectangleF frame) : base (frame)
			{
				baseballFieldImage = UIImage.FromFile ("Images/baseball.png");
				baseballFieldView = new UIImageView (baseballFieldImage);

				baseballFieldView.ContentMode = UIViewContentMode.ScaleAspectFit;
				baseballFieldView.Frame = UIScreen.MainScreen.Bounds;
				AddSubview (baseballFieldView);
			}
		}
	}
}

