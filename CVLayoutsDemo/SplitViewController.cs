using System;
using MonoTouch.UIKit;
using MonoTouch.CoreFoundation;
using System.Drawing;

namespace CVLayoutsDemo
{
	public class SplitViewContoller : UISplitViewController
	{
		MasterController masterController;
		BaseballPlayerController layoutController;
		SplitDelegate splitDelegate;

		UICollectionViewFlowLayout flowLayout;
		DecoratedFlowLayout decoratedFlowLayout;
		BaseballLayout baseballLayout;
		
		public SplitViewContoller () : base()
		{
			splitDelegate = new SplitDelegate ();
			Delegate = splitDelegate;
			
			masterController = new MasterController ();

			flowLayout = new UICollectionViewFlowLayout (){
				HeaderReferenceSize = new SizeF (UIScreen.MainScreen.Bounds.Width, 50),
				SectionInset = new UIEdgeInsets (20,20,20,20),
				ItemSize = new SizeF (100, 100)
			};
	
			baseballLayout = new BaseballLayout (){
				ItemSize = new SizeF (100, 100)
			};

			decoratedFlowLayout = new DecoratedFlowLayout () {
				HeaderReferenceSize = new SizeF (UIScreen.MainScreen.Bounds.Width, 50),
				SectionInset = new UIEdgeInsets (25,100,25,100),
				MinimumInteritemSpacing = 5,
				MinimumLineSpacing = 5,
				ItemSize = new System.Drawing.SizeF (100, 100)
			};
			
			layoutController = new BaseballPlayerController (flowLayout);
			
			masterController.LayoutSelected += (object sender, LayoutSelectedEventArgs e) => {

				if (e.LayoutName == "Flow Layout") {
				
					layoutController.CollectionView.SetCollectionViewLayout (flowLayout, true);
					layoutController.CollectionView.SetContentOffset(new System.Drawing.PointF(0,0), false);

				} else if (e.LayoutName == "Baseball Layout") {

					layoutController.CollectionView.SetCollectionViewLayout (baseballLayout, true);
				} else if (e.LayoutName == "Decorated Layout") {
				
					layoutController.CollectionView.SetCollectionViewLayout (decoratedFlowLayout, true);
					layoutController.CollectionView.SetContentOffset(new PointF(0,0), false);
				}
			};

			ViewControllers = new UIViewController[] {
				masterController,
				layoutController
			};
		}
		
		class SplitDelegate : UISplitViewControllerDelegate
		{
			public override bool ShouldHideViewController (UISplitViewController svc, UIViewController viewController, 
			                                               UIInterfaceOrientation inOrientation)
			{ 
				return false;
				//return true;
			}
		}
		
	}
}