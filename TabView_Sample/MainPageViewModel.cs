using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Core.Preview;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using System.ComponentModel;

namespace TabView_Sample
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<TabItemInfo> TabViewItems { get;  } = new ObservableCollection<TabItemInfo>();

        public bool IsMinTabCount { get => TabViewItems.Count <=1; }
        public bool IsMaxTabCount { get => TabViewItems.Count >= 10; }

        private Thickness _FooterMargin;
        public Thickness FooterMargin
        {
            get => _FooterMargin;
            set
            {
                if(_FooterMargin != value)
                {
                    _FooterMargin = value;
                    RaisePropertyChanged(nameof(FooterMargin));
                }
            }
        } 

        public MainPageViewModel()
        {
            AddTabView(DateTime.Now.ToString(), GetContentPageFrame());
 
            CoreApplication.GetCurrentView().TitleBar.LayoutMetricsChanged += TitleBar_LayoutMetricsChanged;
            SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += (_, __) => { Application.Current.Exit(); };
        }

        private void TitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            FooterMargin = new Thickness(FooterMargin.Left, FooterMargin.Top, sender.SystemOverlayRightInset, FooterMargin.Bottom);
        }

        public void ContentTabView_AddTabButtonClick(TabView sender, object args)
        {
            if(!IsMaxTabCount)
            {
                AddTabView(DateTime.Now.ToString(), GetContentPageFrame());
            }
        }

        public void ContentTabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            if(!IsMinTabCount)
            {
                CloseTabView((TabItemInfo)args.Item);
            }
        }

        public void ContentTabView_TabDroppedOutside(TabView sender, TabViewTabDroppedOutsideEventArgs args)
        {
            var info = (TabItemInfo)args.Item;
            if (!IsMinTabCount && CloseTabView(info))
            {
                ShowNewWindow(info.Hedder);
            }
        }

        private bool CloseTabView(TabItemInfo info)
        {
            return TabViewItems.Remove(info);
        }

        private void AddTabView(string hedder , Frame content = null)
        {
            TabViewItems.Add(new TabItemInfo()
            {
                Hedder = hedder,
                ContentPage = content
            });
        }

        private Frame GetContentPageFrame(object parameter = null)
        {
            var pageFrame = new Frame();
            pageFrame.Navigate(typeof(ContentPage), parameter ?? DateTime.Now.ToString());

            return pageFrame;
        }

        private async void ShowNewWindow(string content)
        {
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Window.Current.Content = GetContentPageFrame(content);
                Window.Current.Activate();
                newViewId = ApplicationView.GetForCurrentView().Id;
            });

            _ = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }

}
