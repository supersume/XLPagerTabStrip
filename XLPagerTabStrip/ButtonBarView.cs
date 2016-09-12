using System;
using CoreGraphics;
using UIKit;
using Foundation;
using System.Threading;

namespace XLPagerTabStrip
{
    [Register("ButtonBarView")]
    public class ButtonBarView : UICollectionView
    {
        SelectedBarAlignment selectedBarAlignment = SelectedBarAlignment.Center;
        int selectedIndex = 0;

        #region Public properties
        public UIView SelectedBar { get; set; }

        private UIView InitializeSelectBar()
        {
            var frame = new CGRect(0, Frame.Size.Height - SelectedBarHeight, 0, SelectedBarHeight);
            UIView bar = new UIView(frame);
            bar.Layer.ZPosition = 9999;
            return bar;
        }

        nfloat _selectedBarHeight = 4;

        internal nfloat SelectedBarHeight
        {
            get
            {
                return _selectedBarHeight;
            }

            set
            {
                _selectedBarHeight = value;
                UpdateSelectedBarYPosition();
            }
        }
        #endregion

        #region Constructors and initializers
        public ButtonBarView(IntPtr handle) : base(handle)
        {

        }

        public ButtonBarView(NSCoder coder) : base(coder)
        {
            SelectedBar = InitializeSelectBar();
            AddSubview(SelectedBar);
        }

        public ButtonBarView(CGRect frame, UICollectionViewLayout layout) : base(frame, layout)
        {
            SelectedBar = InitializeSelectBar();
            AddSubview(SelectedBar);
        }

        public override void AwakeFromNib()
        {
            // Called when loaded from xib or storyboard.
            SelectedBar = InitializeSelectBar();
            AddSubview(SelectedBar);            
        }
        #endregion
        
        public void MoveToIndex(int toIndex, bool animated, SwipeDirection swipeDirection, PagerScroll pagerScroll)
        {
            selectedIndex = toIndex;
            UpdateSelectedBarPosition(animated, swipeDirection: swipeDirection, pagerScroll: pagerScroll);
        }

        public void MoveFromIndex(int fromIndex, int toIndex, nfloat progressPercentage, PagerScroll pagerScroll)
        {
            selectedIndex = progressPercentage > 0.5 ? toIndex : fromIndex;
            
            CGRect fromFrame = GetLayoutAttributesForItem(NSIndexPath.FromItemSection(fromIndex, 0)).Frame;
            var numberOfItems = DataSource.GetItemsCount(this, 0);

            CGRect toFrame;

            if (toIndex < 0 || toIndex > numberOfItems - 1)
            {
                if (toIndex < 0)
                {
                    var cellAtts = GetLayoutAttributesForItem(NSIndexPath.FromItemSection(0, 0));
                    toFrame = CGRect.Inflate(cellAtts.Frame, -cellAtts.Frame.Size.Width, 0);
                }
                else
                {
                    var cellAtts = GetLayoutAttributesForItem(NSIndexPath.FromItemSection((numberOfItems - 1), 0));
                    toFrame = CGRect.Inflate(cellAtts.Frame, cellAtts.Frame.Size.Width, 0);
                }
            }
            else
            {
                toFrame = GetLayoutAttributesForItem(NSIndexPath.FromItemSection(toIndex, 0)).Frame;
            }


            CGRect targetFrame = fromFrame;
            var targetWidth = targetFrame.Size.Width + (toFrame.Size.Width - fromFrame.Size.Width) * progressPercentage;
            targetFrame.Size = new CGSize(targetWidth, SelectedBar.Frame.Size.Height);
            targetFrame.X += (toFrame.X - fromFrame.X) * +(progressPercentage);

            SelectedBar.Frame = new CGRect(targetFrame.X, SelectedBar.Frame.Y, targetFrame.Size.Width, SelectedBar.Frame.Size.Height);

            nfloat targetContentOffset = 0.0f;
            if (ContentSize.Width > Frame.Size.Width)
            {
                var toContentOffset = ContentOffsetForCell(toFrame, toIndex);
                var fromContentOffset = ContentOffsetForCell(fromFrame, fromIndex);

                targetContentOffset = fromContentOffset + ((toContentOffset - fromContentOffset) * progressPercentage);
            }

            var animated = Math.Abs(ContentOffset.X - targetContentOffset) > 30 || (fromIndex == toIndex);
            SetContentOffset(new CGPoint(targetContentOffset, 0), animated: animated);
        }

        public void UpdateSelectedBarPosition(bool animated, SwipeDirection swipeDirection, PagerScroll pagerScroll)
        {
            var selectedBarFrame = SelectedBar.Frame;

            NSIndexPath selectedCellIndexPath = NSIndexPath.FromItemSection(new nint(selectedIndex), 0);
            UICollectionViewLayoutAttributes attributes = GetLayoutAttributesForItem(selectedCellIndexPath);
            CGRect selectedCellFrame = attributes.Frame;

            UpdateContentOffset(animated, pagerScroll: pagerScroll, toFrame: selectedCellFrame, toIndex: selectedCellIndexPath.Row);

            selectedBarFrame.Size = new CGSize(selectedCellFrame.Size.Width, selectedBarFrame.Size.Height);
            selectedBarFrame.X = selectedCellFrame.X;

            if (animated)
            {
                UIView.Animate(0.3, () =>
                {
                    SelectedBar.Frame = selectedBarFrame;
                });
            }
            else
            {
                SelectedBar.Frame = selectedBarFrame;
            }
        }

        #region Helpers
        private void UpdateContentOffset(bool animated, PagerScroll pagerScroll, CGRect toFrame, int toIndex)
        {
            if (pagerScroll != PagerScroll.No || (pagerScroll != PagerScroll.ScrollOnlyIfOutOfScreen && (toFrame.X < ContentOffset.X || toFrame.X >= (ContentOffset.X + Frame.Size.Width - ContentInset.Left))))
            {
                nfloat targetContentOffset = ContentSize.Width > Frame.Size.Width ? ContentOffsetForCell(toFrame, toIndex) : 0;
                SetContentOffset(new CGPoint(targetContentOffset, 0), animated);
            }            
        }

        private nfloat ContentOffsetForCell(CGRect cellFrame, int index)
        {
            var sectionInset = (CollectionViewLayout as UICollectionViewFlowLayout).SectionInset;
            nfloat alignmentOffset = new nfloat(0.0);

            switch (selectedBarAlignment)
            {
                case SelectedBarAlignment.Left:
                    alignmentOffset = sectionInset.Left;
                    break;
                case SelectedBarAlignment.Right:
                    alignmentOffset = Frame.Size.Width - sectionInset.Right - cellFrame.Size.Width;
                    break;
                case SelectedBarAlignment.Center:
                    alignmentOffset = (Frame.Size.Width - cellFrame.Size.Width) * 0.5f;
                    break;
                case SelectedBarAlignment.Progressive:
                    var cellHalfWidth = cellFrame.Size.Width * 0.5f;
                    var leftAlignmentOffset = sectionInset.Left + cellHalfWidth;
                    var rightAlignmentOffset = Frame.Size.Width - sectionInset.Right - cellHalfWidth;
                    var numberOfItems = DataSource.GetItemsCount(this, 0);
                    var progress = index / (numberOfItems - 1);
                    alignmentOffset = leftAlignmentOffset + (rightAlignmentOffset - leftAlignmentOffset) * new nfloat(progress) - cellHalfWidth;
                    break;
            }

            var contentOffset = cellFrame.X - alignmentOffset;
            contentOffset = new nfloat(Math.Max(0, contentOffset));
            contentOffset = new nfloat(Math.Min(ContentSize.Width - Frame.Size.Width, contentOffset));
            return contentOffset;
        }

        private void UpdateSelectedBarYPosition()
        {
            var selectedBarFrame = SelectedBar.Frame;
            selectedBarFrame.Y = Frame.Size.Height - SelectedBarHeight;
            selectedBarFrame.Size = new CGSize(selectedBarFrame.Size.Width, SelectedBarHeight);
            SelectedBar.Frame = selectedBarFrame;
        }

        #endregion
    }

    public enum PagerScroll
    {
        No,
        Yes,
        ScrollOnlyIfOutOfScreen,
    }

    public enum SelectedBarAlignment
    {
        Left,
        Center,
        Right,
        Progressive,
    }
}

