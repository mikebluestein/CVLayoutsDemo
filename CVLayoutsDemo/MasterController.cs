using System;
using System.Linq;
using System.Collections.Generic;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.CoreFoundation;

namespace CVLayoutsDemo
{
	public class MasterController : DialogViewController
	{
		public event EventHandler<LayoutSelectedEventArgs> LayoutSelected;

		List<string> layouts = new List<string>{"Flow Layout", "Decorated Layout", "Baseball Layout"};
		
		public List<string> Layouts {
			get {
				return layouts;
			}
		}
		
		public MasterController () : base (null)
		{
			var layoutSection = new Section ();

			layoutSection.AddAll (
				from layout in layouts
				select (Element)new StringElement (layout, () => {   
				if (LayoutSelected != null)
					LayoutSelected (this, new LayoutSelectedEventArgs{LayoutName = layout});
			}));

			Root = new RootElement ("Layouts") { layoutSection };
		}
	}
	
	public class LayoutSelectedEventArgs : EventArgs
	{
		public string LayoutName { get; set; }
	}
}