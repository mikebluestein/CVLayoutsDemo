using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

using MonoTouch.ObjCRuntime;

namespace CVLayoutsDemo
{
	public class DecoratedFlowLayout : UICollectionViewFlowLayout
	{
		static NSString sectionDecorationViewId = new NSString ("SectionDecorationView");

		Dictionary<int, RectangleF> decorationViewRectangles;

		public DecoratedFlowLayout ()
		{
			RegisterClassForDecorationView (typeof(SectionDecorationView), sectionDecorationViewId);
			decorationViewRectangles = new Dictionary<int, RectangleF>();
		}

		public override void PrepareLayout ()
		{
			base.PrepareLayout ();

			decorationViewRectangles.Clear ();

			// calculate the section rectangles
			// based on shelf layout logic from Mark Pospesel
			// github.com/mpospese/IntroductingCollecitonViews

			int sectionCount = CollectionView.NumberOfSections ();
			int topDecorationPadding = 60;

			float width = CollectionView.ContentSize.Width - (SectionInset.Left + SectionInset.Right);
			int itemsAcross = (int)Math.Floor ((width + MinimumInteritemSpacing)/(ItemSize.Width + MinimumInteritemSpacing));
			float y = 0;

			for (int section = 0; section < sectionCount; section++)
			{
				float h = 0;
				h += HeaderReferenceSize.Height;
				h += SectionInset.Top;

				int itemCount = CollectionView.NumberOfItemsInSection (section);
				int rows = (int) Math.Ceiling (itemCount/(float)itemsAcross);
				for(int row =0; row<rows; row++){
					h+= ItemSize.Height;

					if(row<rows-1)
						h+= MinimumLineSpacing;
				}
				h+= SectionInset.Bottom;

				var r = new RectangleF(SectionInset.Left, y+topDecorationPadding, width, h-15-topDecorationPadding);
				decorationViewRectangles.Add(section, r);

				y += h;
			}
		}

		public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect (RectangleF rect)
		{
			var attribs = base.LayoutAttributesForElementsInRect (rect).ToList ();

			foreach(int key in decorationViewRectangles.Keys){
				var r = decorationViewRectangles[key];
				var decorationAttribs = LayoutAttributesForDecorationView (sectionDecorationViewId, NSIndexPath.FromIndex ((uint)key));
				decorationAttribs.Frame = r;
				decorationAttribs.ZIndex = -1;
				attribs.Add (decorationAttribs);
			}

			return attribs.ToArray ();
		}

		public override UICollectionViewLayoutAttributes LayoutAttributesForDecorationView (NSString kind, NSIndexPath indexPath)
		{
			var decorationAttribs = UICollectionViewLayoutAttributes.CreateForDecorationView<CustomDecorationViewLayoutAttributes> (kind, indexPath);
			UIColor decorationColor;

			if (indexPath.Section % 2 == 0)
				decorationColor = UIColor.Red;
			else
				decorationColor = UIColor.Black;

			decorationAttribs.DecorationColor = decorationColor;

			return decorationAttribs;
		}
	}

	public class SectionDecorationView : UICollectionReusableView
	{
		[Export ("initWithFrame:")]
		public SectionDecorationView (RectangleF frame) : base (frame)
		{
		}

		public override void ApplyLayoutAttributes (UICollectionViewLayoutAttributes layoutAttributes)
		{
			base.ApplyLayoutAttributes (layoutAttributes);

			BackgroundColor = ((CustomDecorationViewLayoutAttributes)layoutAttributes).DecorationColor;
		}
	}

	[Register("CustomCollectionViewLayoutAttributes")]
	public class CustomDecorationViewLayoutAttributes : UICollectionViewLayoutAttributes
	{
		public UIColor DecorationColor { get; set; }

		public override NSObject Copy ()
		{
			var obj = base.Copy () as CustomDecorationViewLayoutAttributes;

			if (obj != null)
				obj.DecorationColor = DecorationColor;

			return obj;
		}
	}
}



