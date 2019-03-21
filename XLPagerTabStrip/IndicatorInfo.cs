using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

namespace XLPagerTabStrip
{
    public struct IndicatorInfo
    {
        public string Title;
        public UIImage Image;
        public UIImage HighlightedImage;
        public string AccessibilityLabel;
        public object UserInfo;

        public IndicatorInfo(string title) : this()
        {
            Title = title;
        }

        public IndicatorInfo(string title, UIImage image, object userInfo) : this()
        {
            Title = title;
            Image = image;
            UserInfo = userInfo;
        }

        public IndicatorInfo(string title, UIImage image, UIImage highlightedImage, object userInfo) : this()
        {
            Title = title;
            AccessibilityLabel = title;
            Image = image;
            HighlightedImage = highlightedImage;
            UserInfo = userInfo;
        }

        public IndicatorInfo(string title, string accessibilityLabel, UIImage image, UIImage highlightedImage, object userInfo) : this()
        {
            Title = title;
            AccessibilityLabel = accessibilityLabel;
            Image = image;
            HighlightedImage = highlightedImage;
            UserInfo = userInfo;
        }
    }
}
