using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace CristalLab {
    public sealed partial class MainWindow:Window {

        public int TabCounter { get; private set; } = 0;

        private TabViewItem CreateDefaultTab() {
            TabViewItem tabViewItem = new() {
                Header = $"Document {++TabCounter}",
                IconSource =  new SymbolIconSource { Symbol = Symbol.Document },
                Content = new WorkspaceRoot()
            };
            return tabViewItem;
        }

        public MainWindow() {
            InitializeComponent();
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(CustomDragRegion);
            CustomDragRegion.MinWidth = 188;
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
