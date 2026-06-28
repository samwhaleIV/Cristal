using CristalLab.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CristalLab {
    public sealed partial class MainWindow:Window {

        public int TabCounter { get; private set; } = 0;

        private TabViewItem CreateDefaultTab() {
            TabViewItem tabViewItem = new() {
                Header = $"Document {++TabCounter}",
                IconSource =  new SymbolIconSource { Symbol = Symbol.Document },
                Content = new TabViewNavView()
            };
            return tabViewItem;
        }

        public MainWindow() {
            InitializeComponent();
            ExtendsContentIntoTitleBar = false;
        }


        private void RootTabView_AddTabButtonClick(TabView sender,object args) {
            TabViewItem newTab = CreateDefaultTab();
            sender.TabItems.Add(newTab);
            sender.SelectedItem = newTab;
        }

        private void RootTabView_TabCloseRequested(TabView sender,TabViewTabCloseRequestedEventArgs args) => sender.TabItems.Remove(args.Tab);

        private void RootTabView_Loaded(object sender,RoutedEventArgs e) {
            var newTab = CreateDefaultTab();
            RootTabView.TabItems.Add(newTab);
            RootTabView.SelectedItem = newTab;
        }
    }
}
