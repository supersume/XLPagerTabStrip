using System;
using System.Collections.Generic;
using System.Text;

namespace XLPagerTabStrip
{
    public class PagerTabStripBehaviour
    {
        public bool? SkipIntermediateViewControllers { get; set; }
        public bool? ElasticIndicatorLimit { get; set; }

        public static PagerTabStripBehaviour Create(bool? skipIntermediateViewControllers, bool? elasticIndicatorLimit = null)
        {
            PagerTabStripBehaviour behaviour = new XLPagerTabStrip.PagerTabStripBehaviour()
            { SkipIntermediateViewControllers = skipIntermediateViewControllers, ElasticIndicatorLimit = elasticIndicatorLimit };

            return behaviour;
        }

        public bool IsProgressiveIndicator
        {
            get { return SkipIntermediateViewControllers != null && ElasticIndicatorLimit != null; }
        }
    }
}
