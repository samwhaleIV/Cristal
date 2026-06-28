using Microsoft.UI.Xaml.Controls;
using System;

namespace CristalLab {
    public abstract partial class WorkspacePage:Page {
        public WorkspaceSession Session => (WorkspaceSession)Frame;
    }
}
