using Cristal;
using Microsoft.UI.Xaml.Controls;

namespace CristalLab {
    public partial class WorkspaceSession:Frame {
        public Continuum Cristal { get; private init; } = new();
    }
}
