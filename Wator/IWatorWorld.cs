using System.Drawing;

namespace VPS.Wator {
  // interface for all implementations of the Wator world simulator
  public interface IWatorWorld {
    void ExecuteStep();
    Bitmap GenerateImage();
  }
}
