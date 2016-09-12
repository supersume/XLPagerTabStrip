using Foundation;
using System;
using UIKit;

namespace XLPagerTabStrip
{
    public partial class ButtonBarViewCell : UICollectionViewCell
    {
        public ButtonBarViewCell (IntPtr handle) : base (handle)
        {
            //Label = InitializeLabel();
        }

        private UILabel InitializeLabel()
        {
            UILabel label = new UILabel(ContentView.Bounds);
            label.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
            label.TextAlignment = UITextAlignment.Center;
            label.Font = UIFont.BoldSystemFontOfSize(14.0f);
            return label;
        }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);
            if (Label.Superview != null)
            {
                ContentView.AddSubview(Label);
            }
        }
    }
}