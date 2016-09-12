using System;
using System.Drawing;

using CoreFoundation;
using UIKit;
using Foundation;
using XLPagerTabStrip;

namespace TabStripExample
{
    [Register("UniversalView")]
    public class UniversalView : UIView
    {
        public UniversalView()
        {
            Initialize();
        }

        public UniversalView(RectangleF bounds) : base(bounds)
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
        UIColor crimson = UIColor.FromRGB(204, 0, 51);
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
            View = new UniversalView();
            View.BackgroundColor = UIColor.White;

            Settings.Style.ButtonBarBackgroundColor = crimson;
            Settings.Style.ButtonBarItemBackgroundColor = crimson;
            Settings.Style.SelectedBarBackgroundColor = UIColor.White;
            Settings.Style.ButtonBarItemFont = UIFont.BoldSystemFontOfSize(12);
            Settings.Style.SelectedBarHeight = 2;
            Settings.Style.ButtonBarMinimumLineSpacing = 0;
            Settings.Style.ButtonBarItemTitleColor = UIColor.White;
            Settings.Style.ButtonBarItemsShouldFillAvailiableWidth = true;
            Settings.Style.ButtonBarLeftContentInset = 0;
            Settings.Style.ButtonBarRightContentInset = 0;
            Settings.Style.ButtonBarHeight = 48;

            ChangeCurrentIndexProgressive = changeCurrentIndexProgressive;

            NavigationItem.SetRightBarButtonItem(new UIBarButtonItem(UIBarButtonSystemItem.Search, (sender, args) => {
                // button was clicked
            }), false);

            NavigationItem.SetLeftBarButtonItem(new UIBarButtonItem(UIBarButtonSystemItem.FixedSpace, (sender, args) => {
                // button was clicked
            }), false);

            base.ViewDidLoad();

            // Perform any additional setup after loading the view
        }

        public override UIViewController[] CreateViewControllersForPagerTabStrip(PagerTabStripViewController pagerTabStripViewController)
        {
            ChildViewController controller1 = new ChildViewController("STREAM");
            ChildViewController controller2 = new ChildViewController("VIDEOS");
            ChildViewController controller3 = new ChildViewController("TOPICS");
            ChildViewController controller4 = new ChildViewController("SOURCES");
            //ChildViewController controller5 = new ChildViewController("Mithra");
            //ChildViewController controller6 = new ChildViewController("Varuna");

            return new UIViewController[] { controller1, controller2, controller3, controller4 };
        }

        void changeCurrentIndexProgressive(ButtonBarViewCell oldCell, ButtonBarViewCell newCell, nfloat progressPercentage, bool changeCurrentIndex, bool animated)
        {
            if (changeCurrentIndex == true)
            {
                //if (oldCell != null)
                //    oldCell.Label.TextColor = UIColor.White;
                //if (newCell != null)
                //    newCell.Label.TextColor = crimson;
            }
        }
    }

    public class ChildViewController : UIViewController, IIndicatorInfoProvider
    {
        public string Title { get; set; }
        public ChildViewController(IntPtr handle) : base(handle) { }
        public ChildViewController(string title)
        {
            Title = title;
        }
        public IndicatorInfo IndicatorInfoForPagerTabStrip(PagerTabStripViewController pagerTabStripController)
        {
            return new IndicatorInfo(Title);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            switch (Title)
            {
                case "STREAM":
                    View.BackgroundColor = UIColor.White;
                    break;
                case "VIDEOS":
                    View.BackgroundColor = UIColor.Blue;
                    break;
                case "TOPICS":
                    View.BackgroundColor = UIColor.Orange; ;
                    break;
                case "SOURCES":
                    View.BackgroundColor = UIColor.Green; ;
                    break;
            }

        }
    }
}