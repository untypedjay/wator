using System.Windows.Forms;

namespace VPS.Wator.Dialogs {
  // Dialog that contains a single property grid through which the parameters of the
  // simulation can be changed.
  public partial class SettingsDialog : Form {
    public SettingsDialog() {
      InitializeComponent();
    }
    public SettingsDialog(Settings settings)
      : this() {
      propertyGrid.SelectedObject = settings;
    }
  }
}