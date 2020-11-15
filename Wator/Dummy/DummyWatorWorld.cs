using System;
using System.Drawing;

namespace VPS.Wator.Dummy {
  // empty class that can be used as a basis for new Wator world implementations
  public class DummyWatorWorld : IWatorWorld {
    public DummyWatorWorld(Settings settings) {
      // Wator world implementations must provide a constructor with a single Settings parameter
    }

    public void ExecuteStep() {
      throw new NotImplementedException("Dummy Wator world cannot be executed.");
    }

    public Bitmap GenerateImage() {
      throw new NotImplementedException("Dummy Wator world cannot be visualized.");
    }
  }
}
