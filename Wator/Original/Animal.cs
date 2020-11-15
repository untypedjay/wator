using System.Drawing;

namespace VPS.Wator.Original {
  // base class for animals (fish & sharks)
  public abstract class Animal {

    // world that this animal lives in
    // an animal can check neighboring cells
    public OriginalWatorWorld World { get; private set; }

    // position of the animal in the world (x/y position)
    public Point Position { get; private set; }

    // age of the animal (only relevant for fish)
    public int Age { get; protected set; }

    // energy of the animal
    // sharks need to eat fish to increase energy
    // energy of a fish is constant
    public int Energy { get; protected set; }

    // boolean flag that indicates wether an animal has moved in the current iteration
    public bool Moved { get; private set; }

    // the color of the enimal (e.g. fish=white, shark=red)
    public abstract Color Color { get; }


    // ctor: create a new animal on the specified position of the given world
    public Animal(OriginalWatorWorld world, Point position) {
      World = world;
      Position = position;
      Age = 0;
      Moved = true;
      Energy = 0;
      // place the new animal in the world
      World.Grid[position.X, position.Y] = this;
    }

    // move the animal to a given position
    // does not check if the position can be reached by the animal
    protected void Move(Point destination) {
      World.Grid[Position.X, Position.Y] = null;
      World.Grid[destination.X, destination.Y] = this;
      Position = destination;
      Moved = true;
    }

    // execute one simulation step for this animal 
    // animal behavior is implemented in the specific classes (fish, shark)
    public abstract void ExecuteStep();

    // commit the current simulation step for this animal
    // resets the moved flag to prepare for the next simulation step
    public virtual void Commit() {
      Moved = false;
    }

    // animals can spawn to create new children
    // specific spawning behaviour of animals is implemented in the specific classes
    protected abstract void Spawn();
  }
}
