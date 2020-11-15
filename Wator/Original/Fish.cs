using System;
using System.Drawing;

namespace VPS.Wator.Original {
  public class Fish : Animal {
    public override Color Color {
      get { return Color.White; }
    }

    // create and initialize a new fish on the specified position of the given world
    public Fish(OriginalWatorWorld world, Point position, int age)
      : base(world, position) {
      Energy = world.InitialFishEnergy;
      Age = age;
    }

    // execute one simulation step for the fish
    // fish move around randomly and spawn when they reach a certain age
    public override void ExecuteStep() {
      // assert that the fish is moved only once in a simulation step
      if (Moved) throw new Exception("Tried to move a fish twice in one time step.");

      Age++;

      Point free = World.SelectNeighbor(null, Position);  // find a random empty neighboring cell
      if (free.X != -1) Move(free);  // empty cell found -> move there

      if (Age >= World.FishBreedTime) Spawn();  // fish reached breeding age -> spawn
    }

    // spawning behaviour of fish
    protected override void Spawn() {
      Point free = World.SelectNeighbor(null, Position);  // find a random empty neighboring cell
      if (free.X != -1) {
        // empty neighboring cell found -> create new fish there
        Fish fish = new Fish(World, free, 0);
        // reduce the age of the parent fish to make sure it is allowed to 
        // reproduce only every FishBreedTime steps
        Age -= World.FishBreedTime;
      }
    }
  }
}
