using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace XLPagerTabStrip
{
    public class PagerTabStripViewController : UIViewController, IUIScrollViewDelegate
    {
        public PagerTabStripViewController()
        {
            
        }
        public PagerTabStripViewController(IntPtr handle) : base (handle)
        {
            //DataSource = this as IPagerTabStripDataSource;
            //Delegate = this as IPagerTabStripDelegate;
        }
        public PagerTabStripViewController(NSCoder coder) : base(coder)
        {

        }

        public PagerTabStripViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {

        }


        public IPagerTabStripDelegate Delegate;
        public IPagerTabStripDataSource DataSource;
        
        public UIScrollView ContainerView { get; set; }

        private UIScrollView InitializeContainerView()
        {
            var containerView = new UIScrollView(new CGRect(0, 0, View.Bounds.Width, View.Bounds.Height));
            containerView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
            return containerView;
        }

        public PagerTabStripBehaviour PagerBehaviour = PagerTabStripBehaviour.Create(true, false);

        public UIViewController[] ViewControllers { get; private set; } = new UIViewController[] { };

        public uint CurrentIndex { get; private set; } = 0;

        public nfloat PageWidth
        {
            get { return ContainerView.Bounds.Width; }
        }

        public nfloat ScrollPercentage
        {
            get
            {
                if (SwipeDirection != SwipeDirection.Right)
                {
                    nfloat module = ContainerView.ContentOffset.X % PageWidth;
                    return new nfloat(module == 0.0 ? 1.0 : module / PageWidth);
                }

                return 1 - fmod(ContainerView.ContentOffset.X >= 0 ? ContainerView.ContentOffset.X : PageWidth + ContainerView.ContentOffset.X, PageWidth) / PageWidth;
                return 1 - (ContainerView.ContentOffset.X >= 0 ? ContainerView.ContentOffset.X : PageWidth + ContainerView.ContentOffset.X % PageWidth) / PageWidth;
            }
        }

        private nfloat fmod(nfloat first, nfloat second)
        {
            return first % second;
        }

        public SwipeDirection SwipeDirection
        {
            get
            {
                if (ContainerView.ContentOffset.X > lastContentOffset)
                    return SwipeDirection.Left;
                else if (ContainerView.ContentOffset.X < lastContentOffset)
                    return SwipeDirection.Right;

                return SwipeDirection.None;
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (ContainerView == null)
                ContainerView = InitializeContainerView();

            if (ContainerView.Superview == null)
            {
                View.AddSubview(ContainerView);
            }

            ContainerView.Bounces = true;
            ContainerView.AlwaysBounceHorizontal = true;
            ContainerView.AlwaysBounceVertical = false;
            ContainerView.ScrollsToTop = false;
            ContainerView.Delegate = this;
            ContainerView.ShowsVerticalScrollIndicator = false;
            ContainerView.ShowsHorizontalScrollIndicator = false;
            ContainerView.PagingEnabled = true;
            ReloadViewControllers();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            isViewAppearing = true;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            lastSize = ContainerView.Bounds.Size;
            UpdateIfNeeded();
            isViewAppearing = false;
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            UpdateIfNeeded();
        }

        public void MoveToViewControllerAtIndex(uint index, bool animated = true)
        {
            if (!(IsViewLoaded && View.Window != null))
            {
                CurrentIndex = index;
                return;
            }

            if (animated && PagerBehaviour.SkipIntermediateViewControllers == true && Math.Abs(CurrentIndex - index) > 1)
            {
				// create a copy of the view controller array so the 
				// orignal view controller array indices don't get modified
				var tmpViewControllers = ViewControllers.ToArray();
                var currentChildVC = ViewControllers[CurrentIndex];
                var fromIndex = CurrentIndex < index ? index - 1 : index + 1;
                var fromChildVC = ViewControllers[fromIndex];
                tmpViewControllers[CurrentIndex] = fromChildVC;
                tmpViewControllers[fromIndex] = currentChildVC;
                pagerTabStripChildViewControllersForScrolling = tmpViewControllers;
                ContainerView.SetContentOffset(new CGPoint(PageOffsetForChildIndex(index: fromIndex), 0), animated: false);
                (NavigationController?.View ?? View).UserInteractionEnabled = false;
                ContainerView.SetContentOffset(new CGPoint(PageOffsetForChildIndex(index: index), 0), animated: true);
            }
            else
            {
                (NavigationController?.View ?? View).UserInteractionEnabled = false;
                ContainerView.SetContentOffset(new CGPoint(PageOffsetForChildIndex(index: index), 0), animated: animated);
            }
        }

        public void MoveToViewController(UIViewController viewController, bool animated)
        {
            MoveToViewControllerAtIndex((uint)Array.IndexOf(ViewControllers, viewController), animated);
        }

        #region PagerTabStripDataSource
        public virtual UIViewController[] CreateViewControllersForPagerTabStrip(PagerTabStripViewController pagerTabStripViewController)
        {
            //assertionFailure("Sub-class must implement the PagerTabStripDataSource viewControllersForPagerTabStrip: method")
            return new UIViewController[] { };
        }

        public virtual UIViewController[] ViewControllersForPagerTabStrip(PagerTabStripViewController pagerTabStripViewController)
        {
            return CreateViewControllersForPagerTabStrip(pagerTabStripViewController);
        }
        #endregion

        #region Helpers
        public void UpdateIfNeeded()
        {
            if (IsViewLoaded && !lastSize.Equals(ContainerView.Bounds.Size))
            {
                UpdateContent();
            }
        }

        public bool CanMoveToIndex(nint index)
        {
            return CurrentIndex != index && ViewControllers.Count() > index;
        }

        public nfloat PageOffsetForChildIndex(uint index)
        {
            return new nfloat(index) * ContainerView.Bounds.Width;
        }

        public nfloat OffsetForChildIndex(uint index)
        {
            return (new nfloat(index) * ContainerView.Bounds.Width) + ((ContainerView.Bounds.Width) - View.Bounds.Width) * 0.5f;
        }

        public nfloat OffsetForChildViewController(UIViewController viewController)
        {
            if (!ViewControllers.Contains(viewController))
            {
                //Throw exception view controller not contained in tab strip
                throw new ViewControllerNotFoundException("The view controller is not contained in the tab strip");
            }
            else
            {
                int index = Array.IndexOf(ViewControllers, viewController);
                return OffsetForChildIndex((uint)index);
            }
        }

        public int PageForContentOffset(nfloat contentOffset)
        {
            var result = VirtualPageForContentOffset(contentOffset);
            return PageForVirtualPage(result);
        }

        public int VirtualPageForContentOffset(nfloat contentOffset)
        {
            nfloat x = (contentOffset + 1.5f * PageWidth) / PageWidth;
            return Convert.ToInt32(x - 1.5);  //Changed 1 to 1.5 due to a yet unconfirmed difference between Int() swift method and Convert.ToInt32() .NET method
        }

        public int PageForVirtualPage(int virtualPage)
        {
            if (virtualPage < 0)
            {
                return 0;
            }
            if (virtualPage > ViewControllers.Count() - 1)
            {
                return ViewControllers.Count() - 1;
            }

            return virtualPage;
        }

        public void UpdateContent()
        {
            if (lastSize.Width != ContainerView.Bounds.Size.Width)
            {
                lastSize = ContainerView.Bounds.Size;
                ContainerView.ContentOffset = new CGPoint(PageOffsetForChildIndex(CurrentIndex), 0);
            }

            lastSize = ContainerView.Bounds.Size;

            var pagerViewControllers = pagerTabStripChildViewControllersForScrolling ?? ViewControllers;
            ContainerView.ContentSize = new CGSize(ContainerView.Bounds.Width * new nfloat(pagerViewControllers.Count()), ContainerView.ContentSize.Height);

            int pagerViewControllersCount = pagerViewControllers.Count();
            for (int index = 0; index < pagerViewControllersCount; index++)
            {
                UIViewController childController = pagerViewControllers[index];
                var pageOffsetForChild = PageOffsetForChildIndex((uint)index);

                if (Math.Abs(ContainerView.ContentOffset.X - pageOffsetForChild) < ContainerView.Bounds.Width)
                {
                    if (childController.ParentViewController != null)
                    {
                        childController.View.Frame = new CGRect(OffsetForChildIndex((uint)index), 0, View.Bounds.Width, ContainerView.Bounds.Height);
                        childController.View.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
                    }
                    else
                    {
                        AddChildViewController(childController);
                        childController.BeginAppearanceTransition(true, animated: false);
                        childController.View.Frame = new CGRect(OffsetForChildIndex((uint)index), 0, View.Bounds.Width, ContainerView.Bounds.Height);
                        childController.View.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
                        ContainerView.AddSubview(childController.View);
                        childController.DidMoveToParentViewController(this);
                        childController.EndAppearanceTransition();
                    }
                }
                else
                {
                    if (childController.ParentViewController != null)
                    {
                        childController.WillMoveToParentViewController(null);
                        childController.BeginAppearanceTransition(false, animated: false);
                        childController.View.RemoveFromSuperview();
                        childController.RemoveFromParentViewController();
                        childController.EndAppearanceTransition();
                    }
                }
            }

            var oldCurrentIndex = CurrentIndex;
            var virtualPage = VirtualPageForContentOffset(ContainerView.ContentOffset.X);
            var newCurrentIndex = PageForVirtualPage(virtualPage);
            CurrentIndex = (uint)newCurrentIndex;
            var changeCurrentIndex = newCurrentIndex != oldCurrentIndex;

            IPagerTabStripIsProgressiveDelegate progressiveDelegate = this as IPagerTabStripIsProgressiveDelegate;
            if (progressiveDelegate != null && PagerBehaviour.IsProgressiveIndicator)
            {
                Tuple<int, int, nfloat> data = ProgressiveIndicatorData((uint)virtualPage);
                progressiveDelegate.PagerTabStripViewController(this, data.Item1, data.Item2, data.Item3, changeCurrentIndex);
            }
            else
            {
                Delegate?.PagerTabStripViewController(this, Math.Min((int)oldCurrentIndex, pagerViewControllers.Count() - 1), (int)newCurrentIndex);
            }
        }

        public virtual void ReloadPagerTabStripView()
        {
            if (IsViewLoaded)
            {
                foreach (UIViewController childController in ViewControllers)
                {
                    if (childController.ParentViewController != null)
                    {
                        childController.View.RemoveFromSuperview();
                        childController.WillMoveToParentViewController(null);
                        childController.RemoveFromParentViewController();
                    }
                }

                ReloadViewControllers();
                ContainerView.ContentSize = new CGSize(ContainerView.Bounds.Width * new nfloat(ViewControllers.Count()), ContainerView.ContentSize.Height);
                if (CurrentIndex >= ViewControllers.Count())
                    CurrentIndex = (uint)ViewControllers.Count() - 1;
                ContainerView.ContentOffset = new CGPoint(PageOffsetForChildIndex(CurrentIndex), 0);
                UpdateContent();
            }
        }
        #endregion

        #region UIScrollDelegate
        [Foundation.Export("scrollViewDidScroll:")]
        public void Scrolled(UIScrollView scrollView)
        {
            if (ContainerView == scrollView)
                UpdateContent();
        }

        [Foundation.Export("scrollViewWillBeginDragging:")]
        public void ScrollViewWillBeginDragging(UIScrollView scrollView)
        {
            if (ContainerView == scrollView)
            {
                lastPageNumber = PageForContentOffset(scrollView.ContentOffset.X);
                lastContentOffset = scrollView.ContentOffset.X;
            }
        }

        [Foundation.Export("scrollViewDidEndScrollingAnimation:")]
        public virtual void ScrollViewDidEndScrollingAnimation(UIScrollView scrollView)
        {
            if (ContainerView == scrollView)
            {
                pagerTabStripChildViewControllersForScrolling = null;
                (NavigationController?.View ?? View).UserInteractionEnabled = true;
                UpdateContent();
            }
        }
        #endregion

        #region Orientation
        public override void ViewWillTransitionToSize(CGSize toSize, IUIViewControllerTransitionCoordinator coordinator)
        {
            base.ViewWillTransitionToSize(toSize, coordinator);
            isViewRotating = true;
            pageBeforeRotate = CurrentIndex;

            coordinator.AnimateAlongsideTransition((context) =>
            {
                if (this != null)
                {
                    isViewRotating = false;
                    CurrentIndex = pageBeforeRotate;
                    UpdateIfNeeded();
                }
            }, (context) => { });
        }
        #endregion

        #region Private methods
        private Tuple<int, int, nfloat> ProgressiveIndicatorData(uint virtualPage)
        {
            var count = ViewControllers.Count();
            int fromIndex = (int)CurrentIndex;
            int toIndex = (int)CurrentIndex;
            SwipeDirection direction = SwipeDirection;

            if (direction == SwipeDirection.Left)
            {
                if (virtualPage > count - 1)
                {
                    fromIndex = count - 1;
                    toIndex = count;
                }
                else
                {
                    if (ScrollPercentage >= 0.5)
                        fromIndex = Math.Max(toIndex - 1, 0);
                    else
                        toIndex = fromIndex + 1;
                }
            }
            else if (direction == SwipeDirection.Right)
            {
                if (virtualPage < 0)
                {
                    fromIndex = 0;
                    toIndex = -1;
                }
                else
                {
                    if (ScrollPercentage > 0.5)
                        fromIndex = Math.Min(toIndex + 1, count - 1);
                    else
                        toIndex = fromIndex - 1;
                }
            }

            var scrollPercentage = PagerBehaviour.ElasticIndicatorLimit == true ? ScrollPercentage : ((toIndex < 0 || toIndex >= count) ? 0.0 : ScrollPercentage);
            return new Tuple<int, int, nfloat>(fromIndex, toIndex, new nfloat(scrollPercentage));
        }

        private void ReloadViewControllers()
        {
            if (DataSource == null)
                throw new NullReferenceException("DataSource cannot be null");
            else
            {
                ViewControllers = DataSource.ViewControllersForPagerTabStrip(this);
                // viewControllers
                if (ViewControllers.Count() < 1)
                    throw new Exception("ViewControllersForPagerTabStrip should provide at least one child view controller");

                foreach (UIViewController childController in ViewControllers)
                {
                    if (!(childController is IIndicatorInfoProvider))
                        throw new Exception("Every view controller provided by PagerTabStripDataSource's viewControllersForPagerTabStrip method must conform to  InfoProvider");
                }
            }
        }
        #endregion

        #region Private variables
        private UIViewController[] pagerTabStripChildViewControllersForScrolling;
        private int lastPageNumber = 0;
        private nfloat lastContentOffset = 0.0f;
        private uint pageBeforeRotate = 0;
        private CGSize lastSize = new CGSize(0, 0);
        internal bool isViewRotating = false;
        internal bool isViewAppearing = false;
        #endregion
    }

    #region Interfaces
    public interface IIndicatorInfoProvider
    {
        IndicatorInfo IndicatorInfoForPagerTabStrip(PagerTabStripViewController pagerTabStripController);
    }

    public interface IPagerTabStripDelegate
    {
        void PagerTabStripViewController(PagerTabStripViewController pagerTabStripViewController, int fromIndex, int toIndex);
    }

    public interface IPagerTabStripIsProgressiveDelegate : IPagerTabStripDelegate
    {
        void PagerTabStripViewController(PagerTabStripViewController pagerTabStripViewController, int fromIndex, int toIndex, nfloat progressPercentage, bool indexWasChanged);
    }

    public interface IPagerTabStripDataSource
    {
        UIViewController[] ViewControllersForPagerTabStrip(PagerTabStripViewController pagerTabStripController);
    }
    #endregion
}
