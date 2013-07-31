using System;
using System.Drawing;
using MonoTouch.Dialog.Utilities;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace CVLayoutsDemo
{
	public class BaseballPlayerController : UICollectionViewController
	{
		static NSString cellId = new NSString ("ImageCell");
		static readonly NSString headerId = new NSString ("Header");

		public BaseballPlayers Team1 { get; private set; }
		public BaseballPlayers Team2 { get; private set; }
		
		public BaseballPlayerController (UICollectionViewLayout layout) : base (layout)
		{	
			CollectionView.ContentSize = UIScreen.MainScreen.Bounds.Size;
			CollectionView.BackgroundColor = UIColor.White;

			Team1 = new BaseballPlayers (Team.One);
			Team2 = new BaseballPlayers (Team.Two);
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			CollectionView.RegisterClassForCell (typeof(BaseballPlayerCell), cellId);

			CollectionView.AddGestureRecognizer (new UITapGestureRecognizer (() => { 

				var layout = CollectionView.CollectionViewLayout as BaseballLayout;
				if(layout != null){
					BaseballLayout animateToLayout = new BaseballLayout (){
						ItemSize = new SizeF (100, 100),
						IsHomeTeamOnField = !layout.IsHomeTeamOnField
					};
				
					CollectionView.SetCollectionViewLayout (animateToLayout, true);
				}

			}));

			CollectionView.RegisterClassForSupplementaryView (typeof(Header), UICollectionElementKindSection.Header, headerId);
		}
		
		public override int GetItemsCount (UICollectionView collectionView, int section)
		{
			if (section == 0)
				return Team1.Count;
			else
				return Team2.Count;
		}

		public override int NumberOfSections (UICollectionView collectionView)
		{
			return 2;
		}
		
		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var imageCell = (BaseballPlayerCell)collectionView.DequeueReusableCell (cellId, indexPath);

			if(indexPath.Section == 0)
				imageCell.UpdateImage (Team1 [indexPath.Row].ImageFile);
			else
				imageCell.UpdateImage (Team2 [indexPath.Row].ImageFile);

			imageCell.PlayerBio = "Lorem ipsum dolor sit amet";

			return imageCell;
		}

		public override UICollectionReusableView GetViewForSupplementaryElement (UICollectionView collectionView, NSString elementKind, NSIndexPath indexPath)
		{
			// get a Header instance to use for the supplementary view
			var headerView = (Header)collectionView.DequeueReusableSupplementaryView (elementKind, headerId, indexPath);
			headerView.Text = String.Format ("Team {0}", indexPath.Section+1);
			return headerView;
		}
	}

	// class to use for supplementary view
	class Header : UICollectionReusableView
	{
		UILabel label;

		public string Text {
			set {
				label.Text = value;
			}
		}

		[Export ("initWithFrame:")]
		public Header (System.Drawing.RectangleF frame) : base (frame)
		{
			label = new UILabel (){
				Frame = new RectangleF (0,0,UIScreen.MainScreen.Bounds.Width, 50),  
				BackgroundColor = UIColor.LightGray,
				TextColor = UIColor.White,
				TextAlignment = UITextAlignment.Left,
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth
			};

			AddSubview (label);
		}
	}
	
}