using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XBMC_WebBrowser
{
    public class ListBoxEntry
    {
        public String url;
        public String title;

        public override string ToString()
        {
            return title;
        }
    }
}
