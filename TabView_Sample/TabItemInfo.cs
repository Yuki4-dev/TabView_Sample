using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace TabView_Sample
{
    public class TabItemInfo
    {
        public string Hedder { get; set; }
        public Frame ContentPage { get; set; }
        public TabItemInfo()   { }
    }
}
