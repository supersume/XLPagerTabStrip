using System;
using System.Collections.Generic;
using System.Text;

namespace XLPagerTabStrip
{
    public class ViewControllerNotFoundException : Exception
    {
        public ViewControllerNotFoundException()
        {
        }

        public ViewControllerNotFoundException(string message)
            : base(message)
        {
        }

        public ViewControllerNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
