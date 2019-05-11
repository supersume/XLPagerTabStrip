using System;
using System.Drawing;

using CoreFoundation;
using UIKit;
using Foundation;
using XLPagerTabStrip;
using CoreGraphics;

namespace TabStripExample
{
	[Register("UniversalView")]
	public class UniversalView : UIView
	{
		public UniversalView()
		{
			Initialize();
		}

		public UniversalView(CGRect bounds) : base(bounds)
		{
			Initialize();
		}

		void Initialize()
		{
			BackgroundColor = UIColor.White;
		}
	}

	[Register("ViewController")]
	public class ViewController : ButtonBarPagerTabStripViewController/*, IPagerTabStripDataSource*/
	{
		UIColor themeColor = UIColor.FromRGB(165, 16, 129);
		public ViewController()
		{

		}

		public ViewController(IntPtr handle)
			: base(handle)
		{
		}

		public ViewController(NSCoder coder) : base(coder)
		{

		}

		public ViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
		{

		}

		public override void DidReceiveMemoryWarning()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning();

			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad()
		{
            View = new UniversalView(View.Frame)
            {
                BackgroundColor = UIColor.FromRGB(214, 214, 214)
            };

            Settings.Style.ButtonBarBackgroundColor = themeColor;
			Settings.Style.ButtonBarItemBackgroundColor = themeColor;
			Settings.Style.SelectedBarBackgroundColor = UIColor.White;
			Settings.Style.ButtonBarItemFont = UIFont.BoldSystemFontOfSize(12);
			Settings.Style.SelectedBarHeight = 2;
            Settings.Style.SelectedBarWidth = 40;
            Settings.Style.ButtonBarMinimumLineSpacing = 0;
			Settings.Style.ButtonBarItemTitleColor = UIColor.White;
			Settings.Style.ButtonBarItemsShouldFillAvailiableWidth = true;
			Settings.Style.ButtonBarLeftContentInset = 0;
			Settings.Style.ButtonBarRightContentInset = 0;
			Settings.Style.ButtonBarHeight = 48;

			ChangeCurrentIndexProgressive = changeCurrentIndexProgressive;

			base.ViewDidLoad();

			// Perform any additional setup after loading the view
		}

		public override UIViewController[] CreateViewControllersForPagerTabStrip(PagerTabStripViewController pagerTabStripViewController)
		{
			ChildViewController controller1 = new ChildViewController("PODCASTS");
			ChildViewController controller2 = new ChildViewController("VIDEOS");
			ChildViewController controller3 = new ChildViewController("TRENDING");
			ChildViewController controller4 = new ChildViewController("PROFILE");

			return new UIViewController[] { controller1, controller2, controller3, controller4 };
		}

		void changeCurrentIndexProgressive(ButtonBarViewCell oldCell, ButtonBarViewCell newCell, nfloat progressPercentage, bool changeCurrentIndex, bool animated)
		{
			//if (changeCurrentIndex == true)
			//{
			//    if (oldCell != null)
			//        oldCell.Label.TextColor = UIColor.White;
			//    if (newCell != null)
			//        newCell.Label.TextColor = crimson;
			//}
		}
	}

	public class ChildViewController : UIViewController, IIndicatorInfoProvider
	{
		public string Header { get; set; }
		public ChildViewController(IntPtr handle) : base(handle) { }
		public ChildViewController(string header)
		{
            Header = header;
		}
		public IndicatorInfo IndicatorInfoForPagerTabStrip(PagerTabStripViewController pagerTabStripController)
		{
			return new IndicatorInfo(Header);
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			switch (Header)
			{
				case "PODCASTS":
					View.BackgroundColor = UIColor.White;
					break;
				case "VIDEOS":
					View.BackgroundColor = UIColor.Blue;
					break;
				case "TRENDING":
					View.BackgroundColor = UIColor.Orange; ;
					break;
				case "PROFILE":
					View.BackgroundColor = UIColor.Green; ;
					break;
			}

		}
	}


    [Register("SegmentedExampleViewController")]
    public class SegmentedExampleViewController : SegmentedPagerTabStripViewController
    {
        UIColor themeColor = UIColor.White;
        public SegmentedExampleViewController()
        {

        }

        public SegmentedExampleViewController(IntPtr handle)
            : base(handle)
        {
        }

        public SegmentedExampleViewController(NSCoder coder) : base(coder)
        {

        }

        public SegmentedExampleViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {

        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            View = new UniversalView(View.Frame)
            {
                BackgroundColor = UIColor.FromRGB(214, 214, 214)
            };

            Settings.Style.SegmentedControlColor = themeColor;
            Settings.Style.SegmentedWidth = UIScreen.MainScreen.Bounds.Width - 60;
            Settings.Style.SegmentedHeight = 35;

            base.ViewDidLoad();

            // Perform any additional setup after loading the view
        }

        public override UIViewController[] CreateViewControllersForPagerTabStrip(PagerTabStripViewController pagerTabStripViewController)
        {
            ChildViewController controller1 = new ChildViewController("PODCASTS");
            ChildViewController controller2 = new ChildViewController("VIDEOS");
            ChildViewController controller3 = new ChildViewController("TRENDING");
            ChildViewController controller4 = new ChildViewController("PROFILE");

            return new UIViewController[] { controller1, controller2, controller3, controller4 };
        }
    }

}