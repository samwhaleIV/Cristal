using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Linq;
using CristalLab.Pages;

namespace CristalLab {
    public sealed partial class WorkspaceRoot:NavigationView {

        public bool Initialized { get; private set; } = false;

        public WorkspaceRoot() {
            InitializeComponent();
        }

        private void Nav_ItemInvoked(NavigationView sender,NavigationViewItemInvokedEventArgs args) {
            var item = sender.MenuItems.OfType<NavigationViewItem>().First(x => (string)x.Content == (string)args.InvokedItem);
            Type? type = item.Tag switch {
                "home" => typeof(HomePage),
                "test" => typeof(TestPage),
                "noise" => typeof(NoiseGenerationTest),
                _ => null,
            };
            if(type != null) {
                ContentFrame.Navigate(type,null,new SuppressNavigationTransitionInfo());
            }
        }

        private void Nav_Loaded(object sender,RoutedEventArgs e) {
            if(Initialized) {
                return;
            }
            foreach(NavigationViewItemBase item in Navigation.MenuItems.Cast<NavigationViewItemBase>()) {
                if(item is NavigationViewItem && item.Tag.ToString() == "home") {
                    Navigation.SelectedItem = item;
                    break;
                }
            }
            ContentFrame.Navigate(typeof(HomePage),null,new SuppressNavigationTransitionInfo());
            Initialized = true;
        }
    }
}
