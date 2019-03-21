using CoreGraphics;
using Foundation;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace XLPagerTabStrip
{
    [Register("ButtonBarPagerTabStripViewController")]
    public class ButtonBarPagerTabStripViewController : PagerTabStripViewController, IPagerTabStripDataSource, IPagerTabStripIsProgressiveDelegate, IUICollectionViewDelegateFlowLayout, IUICollectionViewDelegate, IUICollectionViewDataSource
    {
        #region Properties and variables
        public ButtonBarPagerTabStripSettings Settings = new ButtonBarPagerTabStripSettings();

        public ButtonBarItemSpec<ButtonBarViewCell> ButtonBarItemSpec { get; set; }

        public Action<ButtonBarViewCell, ButtonBarViewCell, bool> ChangeCurrentIndex;
        public Action<ButtonBarViewCell, ButtonBarViewCell, nfloat, bool, bool> ChangeCurrentIndexProgressive;

        [Outlet]
        [GeneratedCode("iOS Designer", "1.0")]
        public ButtonBarView ButtonBarView { get; set; }

        private ButtonBarView InitializeButtonBarView()
        {
            UICollectionViewFlowLayout flowLayout = new UICollectionViewFlowLayout
            {
                ScrollDirection = UICollectionViewScrollDirection.Horizontal
            };
            var buttonBarHeight = new nfloat(Settings.Style.ButtonBarHeight ?? 44f);
            var buttonBar = new ButtonBarView(frame: new CGRect(0, 0, View.Frame.Size.Width, buttonBarHeight), layout: flowLayout)
            {
                BackgroundColor = Settings.Style.ButtonBarBackgroundColor ?? UIColor.Orange
            };
            buttonBar.SelectedBar.BackgroundColor = Settings.Style.SelectedBarBackgroundColor ?? UIColor.Black;
            buttonBar.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            var newContainerViewFrame = new CGRect(ContainerView.Frame.X, buttonBarHeight, ContainerView.Frame.Size.Width, ContainerView.Frame.Size.Height - (buttonBarHeight - ContainerView.Frame.Y));
            ContainerView.Frame = newContainerViewFrame;
            return buttonBar;
        }

        private nfloat GetWidth(IndicatorInfo info)
        {
            UILabel label = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                Font = Settings.Style.ButtonBarItemFont,
                Text = info.Title
            };
            CGSize labelSize = label.IntrinsicContentSize;
            return labelSize.Width + (this?.Settings.Style.ButtonBarItemLeftRightMargin ?? 8) * 2;
        }

        private nfloat?[] cachedCellWidths { get; set; }

        private bool shouldUpdateButtonBarView = true;
        #endregion

        #region Constructors
        public ButtonBarPagerTabStripViewController(IntPtr handle) : base(handle)
        {
            Delegate = this;
            DataSource = this;
        }

        [Export("initWithCoder:")]
        public ButtonBarPagerTabStripViewController()
        {
            Delegate = this;
            DataSource = this;
            Func<IndicatorInfo, nfloat> widthCallback = GetWidth;
            ButtonBarItemSpec = ButtonBarItemSpec<ButtonBarViewCell>.Create(widthCallback, "ButtonCell", NSBundle.FromClass(new ObjCRuntime.Class(typeof(ButtonBarViewCell))));
        }

        public ButtonBarPagerTabStripViewController(NSCoder coder) : base(coder)
        {
            Delegate = this;
            DataSource = this;
        }

        public ButtonBarPagerTabStripViewController(string nibName, NSBundle bundle) : base(nibName, bundle)
        {
            Delegate = this;
            DataSource = this;
        }
        #endregion

        #region Overrides
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (ButtonBarView == null)
                ButtonBarView = InitializeButtonBarView();

            if (cachedCellWidths == null)
                cachedCellWidths = CalculateWidths();            
            
            if (ButtonBarView.Superview == null)
            {
                View.AddSubview(ButtonBarView);
            }
            if (ButtonBarView.Delegate == null)
            {
                ButtonBarView.Delegate = this;
            }
            if (ButtonBarView.DataSource == null)
            {
                ButtonBarView.DataSource = this;
            }
            ButtonBarView.ScrollsToTop = false;
            var flowLayout = ButtonBarView.CollectionViewLayout as UICollectionViewFlowLayout;
            flowLayout.ScrollDirection = UICollectionViewScrollDirection.Horizontal;
            flowLayout.MinimumInteritemSpacing = Settings.Style.ButtonBarMinimumInteritemSpacing ?? flowLayout.MinimumLineSpacing;
            flowLayout.MinimumLineSpacing = Settings.Style.ButtonBarMinimumLineSpacing ?? flowLayout.MinimumLineSpacing;
            var sectionInset = flowLayout.SectionInset;
            flowLayout.SectionInset = new UIEdgeInsets(sectionInset.Top, Settings.Style.ButtonBarLeftContentInset ?? sectionInset.Left,
                sectionInset.Bottom, Settings.Style.ButtonBarRightContentInset ?? sectionInset.Right);

            ButtonBarView.ShowsHorizontalScrollIndicator = false;
            ButtonBarView.BackgroundColor = Settings.Style.ButtonBarBackgroundColor ?? ButtonBarView.BackgroundColor;
            ButtonBarView.SelectedBar.BackgroundColor = Settings.Style.SelectedBarBackgroundColor;

            ButtonBarView.SelectedBarHeight = Settings.Style.SelectedBarHeight ?? ButtonBarView.SelectedBarHeight;

            //register button bar item cell
            if (ButtonBarItemSpec.NibName != null)
                ButtonBarView.RegisterNibForCell(UINib.FromName(ButtonBarItemSpec.NibName, ButtonBarItemSpec.Bundle), "Cell");
            else
                ButtonBarView.RegisterClassForCell(typeof(ButtonBarViewCell), "Cell");
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            ButtonBarView.LayoutIfNeeded();
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            if (isViewAppearing || isViewRotating)
            {
                // Force the UICollectionViewFlowLayout to get laid out again with the new size if
                // a) The view is appearing.  This ensures that
                //    collectionView:layout:sizeForItemAtIndexPath: is called for a second time
                //    when the view is shown and when the view *frame(s)* are actually set
                //    (we need the view frame's to have been set to work out the size's and on the
                //    first call to collectionView:layout:sizeForItemAtIndexPath: the view frame(s)
                //    aren't set correctly)
                // b) The view is rotating.  This ensures that
                //    collectionView:layout:sizeForItemAtIndexPath: is called again and can use the views
                //    *new* frame so that the buttonBarView cell's actually get resized correctly
                cachedCellWidths = CalculateWidths();
                ButtonBarView.CollectionViewLayout.InvalidateLayout();

                // When the view first appears or is rotated we also need to ensure that the barButtonView's
                // selectedBar is resized and its contentOffset/scroll is set correctly (the selected
                // tab/cell may end up either skewed or off screen after a rotation otherwise)
                ButtonBarView.MoveToIndex((int)CurrentIndex, animated: false, swipeDirection: SwipeDirection.None, pagerScroll: PagerScroll.ScrollOnlyIfOutOfScreen);
            }
        }

        public override void ReloadPagerTabStripView()
        {
            base.ReloadPagerTabStripView();
            if (IsViewLoaded)
            {
                ButtonBarView.ReloadData();
                cachedCellWidths = CalculateWidths();
                ButtonBarView.MoveToIndex((int)CurrentIndex, animated: false, swipeDirection: SwipeDirection.None, pagerScroll: PagerScroll.Yes);
            }
        }
        #endregion

            #region Public methods
        public nfloat CalculateStretchedCellWidths(nfloat[] minimumCellWidths, nfloat suggestedStretchedCellWidth, int previousNumberOfLargeCells)
        {
            int numberOfLargeCells = 0;
            nfloat totalWidthOfLargeCells = 0;

            foreach (var minimumCellWidthValue in minimumCellWidths)
            {
                if (minimumCellWidthValue > suggestedStretchedCellWidth)
                {
                    totalWidthOfLargeCells += minimumCellWidthValue;
                    numberOfLargeCells += 1;
                }
            }

            if (numberOfLargeCells > previousNumberOfLargeCells)
            {
                var flowLayout = ButtonBarView.CollectionViewLayout as UICollectionViewFlowLayout;
                var collectionViewAvailiableWidth = ButtonBarView.Frame.Size.Width - flowLayout.SectionInset.Left - flowLayout.SectionInset.Right;
                var numberOfCells = minimumCellWidths.Count();
                var cellSpacingTotal = new nfloat(numberOfCells - 1) * flowLayout.MinimumLineSpacing;

                var numberOfSmallCells = numberOfCells - numberOfLargeCells;
                var newSuggestedStretchedCellWidth = (collectionViewAvailiableWidth - totalWidthOfLargeCells - cellSpacingTotal) / new nfloat(numberOfSmallCells);


                return CalculateStretchedCellWidths(minimumCellWidths, suggestedStretchedCellWidth: newSuggestedStretchedCellWidth, previousNumberOfLargeCells: numberOfLargeCells);
            }
            else
                return suggestedStretchedCellWidth;
        }
        #endregion

        #region IPagerTabStripIsProgressiveDelegate
        public void PagerTabStripViewController(PagerTabStripViewController pagerTabStripViewController, int fromIndex, int toIndex)
        {
            if (shouldUpdateButtonBarView)
            {
                ButtonBarView.MoveToIndex(toIndex, animated: true, swipeDirection: toIndex < fromIndex ? SwipeDirection.Right : SwipeDirection.Left, pagerScroll: PagerScroll.Yes);
                if (ChangeCurrentIndex != null)
                {
                    var oldCell = ButtonBarView.CellForItem(NSIndexPath.FromItemSection(new nint(CurrentIndex != fromIndex ? fromIndex : toIndex), 0)) as ButtonBarViewCell;
                    var newCell = ButtonBarView.CellForItem(NSIndexPath.FromItemSection(new nint(CurrentIndex), 0)) as ButtonBarViewCell;
                    ChangeCurrentIndex(oldCell, newCell, true);
                }
            }
        }

        public void PagerTabStripViewController(PagerTabStripViewController pagerTabStripViewController, int fromIndex, int toIndex, nfloat progressPercentage, bool indexWasChanged)
        {
            if (shouldUpdateButtonBarView)
            {
                ButtonBarView.MoveFromIndex(fromIndex, toIndex: toIndex, progressPercentage: progressPercentage, pagerScroll: PagerScroll.Yes);
                if (ChangeCurrentIndexProgressive != null)
                {
                    var oldCell = ButtonBarView.CellForItem(NSIndexPath.FromItemSection(new nint(CurrentIndex != fromIndex ? fromIndex : toIndex), 0)) as ButtonBarViewCell;
                    var newCell = ButtonBarView.CellForItem(NSIndexPath.FromItemSection(new nint(CurrentIndex), 0)) as ButtonBarViewCell;
                    ChangeCurrentIndexProgressive(oldCell, newCell, progressPercentage, indexWasChanged, true);
                }
            }
        }
        #endregion

        #region IUICollectionViewDelegateFlowLayout
        [Export("collectionView:layout:sizeForItemAtIndexPath:")]
        public CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
        {
            var cellWidthValue = cachedCellWidths?[indexPath.Row];
            if (cellWidthValue == null)
            {
                throw new NullReferenceException($"cachedCellWidths for {indexPath.Row} must not be null");
            }
            return new CGSize(cellWidthValue.Value, collectionView.Frame.Size.Height);
        }
        #endregion

        #region IUICollectionViewDelegate
        [Export("collectionView:didSelectItemAtIndexPath:")]
        public void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            if (indexPath.Item != CurrentIndex)
            {
                ButtonBarView.MoveToIndex((int)indexPath.Item, true, SwipeDirection.None, PagerScroll.Yes);
                shouldUpdateButtonBarView = false;

                var oldCell = ButtonBarView.CellForItem(NSIndexPath.FromItemSection(new nint(CurrentIndex), 0)) as ButtonBarViewCell;
                var newCell = ButtonBarView.CellForItem(NSIndexPath.FromItemSection(indexPath.Item, 0)) as ButtonBarViewCell;

                if (PagerBehaviour.IsProgressiveIndicator)
                {
                    ChangeCurrentIndexProgressive?.Invoke(oldCell, newCell, 1, true, true);
                }
                else
                {
                    ChangeCurrentIndex?.Invoke(oldCell, newCell, true);
                }
                MoveToViewControllerAtIndex((uint)indexPath.Item);
            }
        }
        #endregion

        #region IUICollectionViewDataSource
        public nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return ViewControllers.Count();
        }

        public UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var _cell = collectionView.DequeueReusableCell("Cell", indexPath);// as ButtonBarViewCell;
            if (!(_cell is ButtonBarViewCell))
            {
                throw new Exception("UICollectionViewCell should be or extend from ButtonBarViewCell");
            }

            var cell = _cell as ButtonBarViewCell;
            var childController = ViewControllers[indexPath.Item] as IIndicatorInfoProvider;
            var indicatorInfo = childController.IndicatorInfoForPagerTabStrip(this);

            cell.Label.Text = indicatorInfo.Title;
            cell.Label.Font = Settings.Style.ButtonBarItemFont;
            cell.Label.TextColor = Settings.Style.ButtonBarItemTitleColor ?? cell.Label.TextColor;
            cell.ContentView.BackgroundColor = Settings.Style.ButtonBarItemBackgroundColor ?? cell.ContentView.BackgroundColor;
            cell.BackgroundColor = Settings.Style.ButtonBarItemBackgroundColor ?? cell.BackgroundColor;

            if (indicatorInfo.Image != null)
                cell.ImageView.Image = indicatorInfo.Image;
            if (indicatorInfo.HighlightedImage != null)
                cell.ImageView.HighlightedImage = indicatorInfo.HighlightedImage;

            ConfigureCell(cell, indicatorInfo);
                
            if (PagerBehaviour.IsProgressiveIndicator)
                ChangeCurrentIndexProgressive?.Invoke(CurrentIndex == indexPath.Item ? null : cell, CurrentIndex == indexPath.Item ? cell : null, 1, true, false);
            else
                ChangeCurrentIndex?.Invoke(CurrentIndex == indexPath.Item ? null : cell, CurrentIndex == indexPath.Item ? cell : null, false);

            cell.IsAccessibilityElement = true;
            cell.AccessibilityLabel = indicatorInfo.AccessibilityLabel ?? cell.Label.Text;

            return cell;
        }
        #endregion

        #region IUIScrollViewDelegate
        public override void ScrollViewDidEndScrollingAnimation(UIScrollView scrollView)
        {
            base.ScrollViewDidEndScrollingAnimation(scrollView);
            if (scrollView == ContainerView)
                shouldUpdateButtonBarView = true;
        }
        #endregion

        #region Miscellenious
        public void ConfigureCell(ButtonBarViewCell cell, IndicatorInfo indicatorInfo)
        {

        }
        #endregion

        #region Private methods
        public nfloat?[] CalculateWidths()
        {
            var flowLayout = ButtonBarView.CollectionViewLayout as UICollectionViewFlowLayout;
            var numberOfCells = ViewControllers.Count();

            List<nfloat> minimumCellWidths = new List<nfloat>();
            nfloat collectionViewContentWidth = 0;

            foreach (var viewController in ViewControllers)
            {
                var childController = viewController as IIndicatorInfoProvider;
                var indicatorInfo = childController.IndicatorInfoForPagerTabStrip(this);

                if (ButtonBarItemSpec.NibName == null)
                {
                    nfloat width = width = ButtonBarItemSpec.WidthCallback(indicatorInfo);
                    minimumCellWidths.Add(width);
                    collectionViewContentWidth += width;
                }
                else
                {
                    nfloat width = width = ButtonBarItemSpec.WidthCallback(indicatorInfo);
                    minimumCellWidths.Add(width);
                    collectionViewContentWidth += width;
                }
            }

            var cellSpacingTotal = new nfloat(numberOfCells - 1) * flowLayout.MinimumLineSpacing;
            collectionViewContentWidth += cellSpacingTotal;

            nfloat collectionViewAvailableVisibleWidth = ButtonBarView.Frame.Size.Width - flowLayout.SectionInset.Left - flowLayout.SectionInset.Right;
            if (!Settings.Style.ButtonBarItemsShouldFillAvailiableWidth || collectionViewAvailableVisibleWidth < collectionViewContentWidth)
                return minimumCellWidths.Select(i => (nfloat?)i).ToList().ToArray();
            else
            {
                var stretchedCellWidthIfAllEqual = (collectionViewAvailableVisibleWidth - cellSpacingTotal) / new nfloat(numberOfCells);
                var generalMinimumCellWidth = CalculateStretchedCellWidths(minimumCellWidths.ToArray(), suggestedStretchedCellWidth: stretchedCellWidthIfAllEqual, previousNumberOfLargeCells: 0);

                List<nfloat> stretchedCellWidths = new List<nfloat>();
                foreach (nfloat minimumCellWidthValue in minimumCellWidths)
                {
                    var cellWidth = (minimumCellWidthValue > generalMinimumCellWidth) ? minimumCellWidthValue : generalMinimumCellWidth;
                    stretchedCellWidths.Add(cellWidth);
                }

                return stretchedCellWidths.Select(i => (nfloat?)i).ToList().ToArray();
            }
        }
        #endregion

    }

    public class ButtonBarItemSpec<ButtonBarViewCell>
    {
        //private IndicatorInfo info { get; set; }
        public string NibName { get; set; }
        public NSBundle Bundle { get; set; }
        public Func<IndicatorInfo, nfloat> WidthCallback { get; set; }

        public static ButtonBarItemSpec<ButtonBarViewCell> Create(Func<IndicatorInfo, nfloat> widthCallback, string nibName = null, NSBundle bundle = null)
        {
            ButtonBarItemSpec<ButtonBarViewCell> spec = new ButtonBarItemSpec<ButtonBarViewCell>() { WidthCallback = widthCallback, NibName = nibName, Bundle = bundle };
            return spec;
        }
    }

    public class ButtonBarPagerTabStripSettings
    {
        public ButtonBarPagerTabStripStyle Style { get; set; } = new ButtonBarPagerTabStripStyle();
    }

    public class ButtonBarPagerTabStripStyle
    {
        public UIColor ButtonBarBackgroundColor { get; set; }
        public nfloat? ButtonBarMinimumInteritemSpacing { get; set; }
        public nfloat? ButtonBarMinimumLineSpacing { get; set; }
        public nfloat? ButtonBarLeftContentInset { get; set; }
        public nfloat? ButtonBarRightContentInset { get; set; }

        public UIColor SelectedBarBackgroundColor { get; set; } = UIColor.Black;
        public nfloat? SelectedBarHeight { get; set; } = 5;

        public UIColor ButtonBarItemBackgroundColor { get; set; }
        public UIFont ButtonBarItemFont { get; set; } = UIFont.SystemFontOfSize(18);
        public nfloat? ButtonBarItemLeftRightMargin { get; set; } = 8;
        public UIColor ButtonBarItemTitleColor { get; set; }
        public bool ButtonBarItemsShouldFillAvailiableWidth { get; set; } = true;

        // only used if button bar is created programaticaly and not using storyboards or nib files
        public nfloat? ButtonBarHeight { get; set; }
    }
}
