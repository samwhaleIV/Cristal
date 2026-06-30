using Cristal;
using Microsoft.UI.Xaml.Controls;

namespace CristalLab {
    public partial class WorkspaceSession:Frame {
        public Continuum CristalFactory { get; private init; } = new();
    }
}
