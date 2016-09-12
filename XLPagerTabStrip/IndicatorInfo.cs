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

        public IndicatorInfo(string title) : this()
        {
            Title = title;
        }

        public IndicatorInfo(string title, UIImage image) : this()
        {
            Title = title;
            Image = image;
        }

        public IndicatorInfo(string title, UIImage image, UIImage highlightedImage) : this()
        {
            Title = title;
            Image = image;
            HighlightedImage = highlightedImage;
        }
    }
}
