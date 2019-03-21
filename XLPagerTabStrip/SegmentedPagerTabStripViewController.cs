using System;
using System.CodeDom.Compiler;
using CoreGraphics;
using Foundation;
using UIKit;

namespace XLPagerTabStrip
{
    [Register("SegmentedPagerTabStripViewController")]
    public class SegmentedPagerTabStripViewController : PagerTabStripViewController, IPagerTabStripDataSource, IPagerTabStripIsProgressiveDelegate
    {
        #region Properties and variables
        public SegmentedPagerTabStripSettings Settings = new SegmentedPagerTabStripSettings();

        [Outlet]
        [GeneratedCode("iOS Designer", "1.0")]
        public UISegmentedControl SegmentedControl { get; set; }

        private bool shouldUpdateSegmentedControl = true;
        #endregion

        #region Constructors
        public SegmentedPagerTabStripViewController(IntPtr handle) : base(handle)
        {
            Delegate = this;
            DataSource = this;
        }

        [Export("initWithCoder:")]
        public SegmentedPagerTabStripViewController()
        {
            PagerBehaviour = PagerTabStripBehaviour.Create(true);
            Delegate = this;
            DataSource = this;
        }

        public SegmentedPagerTabStripViewController(NSCoder coder) : base(coder)
        {
            PagerBehaviour = PagerTabStripBehaviour.Create(true);
            Delegate = this;
            DataSource = this;
        }

        public SegmentedPagerTabStripViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
            PagerBehaviour = PagerTabStripBehaviour.Create(true);
            Delegate = this;
            DataSource = this;
        }
        #endregion

        #region Overrides
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (SegmentedControl == null)
            {
                SegmentedControl = new UISegmentedControl(new CGRect(0, 0, Settings.Style.SegmentedWidth, Settings.Style.SegmentedHeight));
            }

            if (SegmentedControl.Superview == null)
            {
                NavigationItem.TitleView = SegmentedControl;
            }

            SegmentedControl.SetTitleTextAttributes(new UITextAttributes() { Font = UIFont.SystemFontOfSize(15) }, UIControlState.Normal);
            SegmentedControl.SetTitleTextAttributes(new UITextAttributes() { Font = UIFont.BoldSystemFontOfSize(15) }, UIControlState.Selected);

            SegmentedControl.TintColor = Settings.Style.SegmentedControlColor ?? SegmentedControl.TintColor;
            SegmentedControl.AddTarget(this, new ObjCRuntime.Selector("segmentedControlChanged:"), UIControlEvent.ValueChanged);
            ReloadSegmentedControl();
        }


        public override void ReloadPagerTabStripView()
        {
            base.ReloadPagerTabStripView();
            if (IsViewLoaded)
            {
                ReloadSegmentedControl();
            }
        }
        #endregion

        #region Helpers
        private void ReloadSegmentedControl()
        {
            SegmentedControl.RemoveAllSegments();

            for (int index = 0; index < ViewControllers.Length; index++)
            {
                var childController = ViewControllers[index] as IIndicatorInfoProvider;
                var indicatorInfo = childController.IndicatorInfoForPagerTabStrip(this);

                if (indicatorInfo.Image != null)
                {
                    SegmentedControl.InsertSegment(indicatorInfo.Image, index, false);
                }
                else
                {
                    SegmentedControl.InsertSegment(indicatorInfo.Title, index, false);
                }
            }

            SegmentedControl.SelectedSegment = (nint)CurrentIndex;
        }

        [Export("segmentedControlChanged:")]
        private void SegmentedControlChanged(UISegmentedControl sender)
        {
            var index = sender.SelectedSegment;
            PagerTabStripViewController(this, (int)CurrentIndex, (int)index);
            shouldUpdateSegmentedControl = false;
            MoveToViewControllerAtIndex((uint)index);
        }
        #endregion

        #region IPagerTabStripIsProgressiveDelegate
        public void PagerTabStripViewController(PagerTabStripViewController pagerTabStripViewController, int fromIndex, int toIndex, nfloat progressPercentage, bool indexWasChanged)
        {
            if (shouldUpdateSegmentedControl)
            {
                SegmentedControl.SelectedSegment = toIndex;
            }
        }

        public void PagerTabStripViewController(PagerTabStripViewController pagerTabStripViewController, int fromIndex, int toIndex)
        {
            if (shouldUpdateSegmentedControl)
            {
                SegmentedControl.SelectedSegment = toIndex;
            }
        }
        #endregion

        #region IUIScrollViewDelegate
        public override void ScrollViewDidEndScrollingAnimation(UIScrollView scrollView)
        {
            base.ScrollViewDidEndScrollingAnimation(scrollView);
            shouldUpdateSegmentedControl = true;
        }
        #endregion

    }

    public class SegmentedPagerTabStripSettings
    {
        public SegmentedPagerTabStripStyle Style { get; set; } = new SegmentedPagerTabStripStyle();
    }

    public class SegmentedPagerTabStripStyle
    {
        public UIColor SegmentedControlColor { get; set; }

        public UIFont SegmentedItemNormalFont { get; set; } = UIFont.SystemFontOfSize(15);
        public UIFont SegmentedItemSelectedFont { get; set; } = UIFont.BoldSystemFontOfSize(18);

        public nfloat SegmentedWidth { get; set; } = UIScreen.MainScreen.Bounds.Width - 40;
        public nfloat SegmentedHeight { get; set; } = 40;
    }
}
