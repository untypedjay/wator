using System;
using System.Drawing;

namespace VPS.Wator.Original {
  public class Shark : Animal {
    public override Color Color {
      get { return Color.Red; }
    }

    // create and initialize a shark on the specified position in the given world
    public Shark(OriginalWatorWorld world, Point position, int energy)
      : base(world, position) {
      Energy = energy;
    }

    // execute one simulation step for this shark
    // sharks try to eat neighbouring fish if possible
    // otherwise they move to a random free neighbouring point
    // if a shark has eaten enough fish it spawns
    public override void ExecuteStep() {
      // assert that a the shark is never moved twice within one simulation step
      if (Moved) throw new Exception("Tried to move a shark twice within one time step.");

      Age++;
      Energy--;

      Point fish = World.SelectNeighbor(typeof(Fish), Position);  // find a random neighboring fish
      if (fish.X != -1) {
        // neighboring fish found -> eat and move to that position
        Energy += World.Grid[fish.X, fish.Y].Energy;
        Move(fish);
      } else {  // no neighboring fish found
        Point free = World.SelectNeighbor(null, Position);  // find a random empty neighboring cell
        if (free.X != -1) Move(free);  // empty cell found -> move there
      }

      if (Energy >= World.SharkBreedEnergy) Spawn();  // enough energy -> spawn
      if (Energy <= 0) World.Grid[Position.X, Position.Y] = null;  // not enough energy -> die
    }

    // spawning behaviour of sharks
    protected override void Spawn() {
      Point free = World.SelectNeighbor(null, Position);  // find a random empty neighboring cell
      if (free.X != -1) {
        // empty neighboring cell found -> create new shark there and share energy between parent and child shark
        Shark shark = new Shark(World, free, Energy / 2);
        Energy = Energy / 2;
      }
    }
  }
}
