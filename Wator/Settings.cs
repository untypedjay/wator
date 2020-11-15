using System.ComponentModel;

namespace VPS.Wator {
  [DefaultPropertyAttribute("Version")]
  // Parameters and default parameter settings for the Wator world simulation. 
  // Settings can be configured in the GUI. The attributes are necessary
  // to make it easy to edit parameter settings using a property grid.
  public class Settings {
    #region General Settings
    [CategoryAttribute("General Settings")]
    public Version Version { get; set; }
    [CategoryAttribute("General Settings")]
    public int Workers { get; set; }
    [CategoryAttribute("General Settings")]
    public int Runs { get; set; }
    [CategoryAttribute("General Settings")]
    public int Iterations { get; set; }
    [CategoryAttribute("General Settings")]
    public int Width { get; set; }
    [CategoryAttribute("General Settings")]
    public int Height { get; set; }
    [CategoryAttribute("General Settings")]
    public bool DisplayWorld { get; set; }
    [CategoryAttribute("General Settings")]
    public int DisplayInterval { get; set; }
    #endregion

    #region Fish Settings
    [CategoryAttribute("Fish Settings")]
    public int InitialFishPopulation { get; set; }
    [CategoryAttribute("Fish Settings")]
    public int InitialFishEnergy { get; set; }
    [CategoryAttribute("Fish Settings")]
    public int FishBreedTime { get; set; }
    #endregion

    #region Shark Settings
    [CategoryAttribute("Shark Settings")]
    public int InitialSharkPopulation { get; set; }
    [CategoryAttribute("Shark Settings")]
    public int InitialSharkEnergy { get; set; }
    [CategoryAttribute("Shark Settings")]
    public int SharkBreedEnergy { get; set; }
    #endregion

    public Settings() {
      Version = Version.OriginalWatorWorld;
      Workers = 1;
      Runs = 5;
      Iterations = 1000;
      Width = 500;
      Height = 500;
      DisplayWorld = true;
      DisplayInterval = 1;
      InitialFishPopulation = 20000;
      InitialFishEnergy = 10;
      FishBreedTime = 10;
      InitialSharkPopulation = 5000;
      InitialSharkEnergy = 25;
      SharkBreedEnergy = 50;
    }
  }
}
