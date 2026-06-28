using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CristalLab.Pages {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TabViewNavView:NavigationView {

        public bool Initialized { get; private set; }

        public TabViewNavView() {
            InitializeComponent();
        }

        private void Navigation_ItemInvoked(NavigationView sender,NavigationViewItemInvokedEventArgs args) {
            var item = sender.MenuItems.OfType<NavigationViewItem>().First(x => (string)x.Content == (string)args.InvokedItem);
            Navigation_Navigate(item);
        }

        private void Navigation_Navigate(NavigationViewItem item) {
            Type? type = item.Tag switch {
                "home" => typeof(TabHomePage),
                "test" => typeof(TestPage),
                "noise" => typeof(NoiseGenerationTest),
                _ => null,
            };
            if(type != null) {
                ContentFrame.Navigate(type,null,new Microsoft.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());
            }
        }

        private void Navigation_Loaded(object sender,RoutedEventArgs e) {
            if(Initialized) {
                return;
            }
            foreach(NavigationViewItemBase item in Navigation.MenuItems.Cast<NavigationViewItemBase>()) {
                if(item is NavigationViewItem && item.Tag.ToString() == "home") {
                    Navigation.SelectedItem = item;
                    break;
                }
            }
            ContentFrame.Navigate(typeof(TabHomePage));
            Initialized = true;
        }
    }
}
