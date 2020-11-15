using System;
using System.Drawing;

namespace VPS.Wator.Original {
  // initial object-oriented implementation of the Wator world simulation
  public class OriginalWatorWorld : IWatorWorld {
    private Random random;

    // A matrix of ints that determines the order of execution for each cell of the world.
    // This matrix is shuffled in each time step.
    // Cells of the world must be executed in a random order,
    // otherwise the animal in the first cell is always allowed to move first.
    private int[,] randomMatrix;

    // for visualization
    private byte[] rgbValues;

    #region Properties
    public int Width { get; private set; }  // width (number of cells) of the world
    public int Height { get; private set; }  // height (number of cells) of the world
    public Animal[,] Grid { get; private set; }  // the cells of the world (2D-array of animal (fish or shark), empty cells have the value null)

    // simulation parameters
    public int InitialFishPopulation { get; private set; }
    public int InitialFishEnergy { get; private set; }
    public int FishBreedTime { get; private set; }

    public int InitialSharkPopulation { get; private set; }
    public int InitialSharkEnergy { get; private set; }
    public int SharkBreedEnergy { get; private set; }
    #endregion

    public OriginalWatorWorld(Settings settings) {
      Width = settings.Width;
      Height = settings.Height;
      InitialFishPopulation = settings.InitialFishPopulation;
      InitialFishEnergy = settings.InitialFishEnergy;
      FishBreedTime = settings.FishBreedTime;
      InitialSharkPopulation = settings.InitialSharkPopulation;
      InitialSharkEnergy = settings.InitialSharkEnergy;
      SharkBreedEnergy = settings.SharkBreedEnergy;

      rgbValues = new byte[Width * Height * 4];

      random = new Random();
      Grid = new Animal[Width, Height];

      // populate the random matrix that determines the order of execution for the cells
      randomMatrix = GenerateRandomMatrix(Width, Height);

      // Initialize the population by placing the required number of shark and fish
      // randomly on the grid.
      // randomMatrix contains all values from 0 .. Width*Height in a random ordering
      // so we can simply place a fish onto a cell if the value in the same cell of the
      // randomMatrix is smaller then the number of fish 
      // subsequently we can place a shark if the number in randomMatrix is smaller than
      // the number of fish and shark
      for (int i = 0; i < Width; i++) {
        for (int j = 0; j < Height; j++) {
          int value = randomMatrix[i, j];
          if (value < InitialFishPopulation) {
            Grid[i, j] = new Fish(this, new Point(i, j), random.Next(0, FishBreedTime));
          } else if (value < InitialFishPopulation + InitialSharkPopulation) {
            Grid[i, j] = new Shark(this, new Point(i, j), random.Next(0, SharkBreedEnergy));
          } else {
            Grid[i, j] = null;
          }
        }
      }

    }

    // Execute one time step of the simulation. Each cell of the world must be executed once.
    // Animals move around on the grid. To make sure each animal is executed only once we
    // use the moved flag.
    public void ExecuteStep() {
      RandomizeMatrix(randomMatrix);  // make sure that order of execution of cells is different and random in each time step

      // process all animals in random order
      int row, col;
      for (int i = 0; i < Width; i++) {
        for (int j = 0; j < Height; j++) {
          // get random position (row/colum) from random matrix
          col = randomMatrix[i, j] % Width;
          row = randomMatrix[i, j] / Width;

          if (Grid[col, row] != null && !Grid[col, row].Moved)  // process unmoved animals
            Grid[col, row].ExecuteStep();
        }
      }

      // commit all animals in the grid to prepare for the next simulation step
      for (int i = 0; i < Width; i++) {
        for (int j = 0; j < Height; j++) {
          if (Grid[i, j] != null)
            Grid[i, j].Commit();
        }
      }
    }

    // generate bitmap for the current state of the Wator world
    public Bitmap GenerateImage() {
      int counter = 0;
      for (int y = 0; y < Height; y++) {
        for (int x = 0; x < Width; x++) {
          Color col;
          if (Grid[x, y] == null) col = Color.DarkBlue;
          else col = Grid[x, y].Color;

          rgbValues[counter++] = col.B; // blue
          rgbValues[counter++] = col.G; // green
          rgbValues[counter++] = col.R; // red
          rgbValues[counter++] = col.A; // alpha
        }
      }

      Rectangle rect = new Rectangle(0, 0, Width, Height);
      var bitmap = new Bitmap(Width, Height);
      System.Drawing.Imaging.BitmapData bmpData = null;
      try {
        // lock the bitmap's bits
        bmpData = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bitmap.PixelFormat);

        // get the address of the first line
        IntPtr ptr = bmpData.Scan0;

        // copy RGB values back to the bitmap
        System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, rgbValues.Length);
      }
      finally {
        // unlock the bits
        if (bmpData != null) bitmap.UnlockBits(bmpData);
      }
      return bitmap;
    }

    // find all neighboring cells of the given position and type
    public Point[] GetNeighbors(Type type, Point position) {
      Point[] neighbors = new Point[4];
      int neighborIndex = 0;
      int i, j;

      // look north
      i = position.X;
      j = (position.Y + Height - 1) % Height;
      if ((type == null) && (Grid[i, j] == null)) {
        neighbors[neighborIndex] = new Point(i, j);
        neighborIndex++;
      } else if ((type != null) && (type.IsInstanceOfType(Grid[i, j]))) {
        if ((Grid[i, j] != null) && (!Grid[i, j].Moved)) {  // ignore animals moved in the current iteration
          neighbors[neighborIndex] = new Point(i, j);
          neighborIndex++;
        }
      }
      // look east
      i = (position.X + 1) % Width;
      j = position.Y;
      if ((type == null) && (Grid[i, j] == null)) {
        neighbors[neighborIndex] = new Point(i, j);
        neighborIndex++;
      } else if ((type != null) && (type.IsInstanceOfType(Grid[i, j]))) {
        if ((Grid[i, j] != null) && (!Grid[i, j].Moved)) {
          neighbors[neighborIndex] = new Point(i, j);
          neighborIndex++;
        }
      }
      // look south
      i = position.X;
      j = (position.Y + 1) % Height;
      if ((type == null) && (Grid[i, j] == null)) {
        neighbors[neighborIndex] = new Point(i, j);
        neighborIndex++;
      } else if ((type != null) && (type.IsInstanceOfType(Grid[i, j]))) {
        if ((Grid[i, j] != null) && (!Grid[i, j].Moved)) {
          neighbors[neighborIndex] = new Point(i, j);
          neighborIndex++;
        }
      }
      // look west
      i = (position.X + Width - 1) % Width;
      j = position.Y;
      if ((type == null) && (Grid[i, j] == null)) {
        neighbors[neighborIndex] = new Point(i, j);
        neighborIndex++;
      } else if ((type != null) && (type.IsInstanceOfType(Grid[i, j]))) {
        if ((Grid[i, j] != null) && (!Grid[i, j].Moved)) {
          neighbors[neighborIndex] = new Point(i, j);
          neighborIndex++;
        }
      }

      // create result array that only contains found cells
      Point[] result = new Point[neighborIndex];
      Array.Copy(neighbors, result, neighborIndex);
      return result;
    }

    // select a random neighboring cell of the given position and type
    public Point SelectNeighbor(Type type, Point position) {
      Point[] neighbors = GetNeighbors(type, position);  // find all neighbors of required type
      if (neighbors.Length > 1) {
        return neighbors[random.Next(neighbors.Length)];  // return random neighbor (prevent bias)
      } else if (neighbors.Length == 1) {  // only one neighbor -> return without calling random
        return neighbors[0];
      } else {
        return new Point(-1, -1);  // no neighbor found
      }
    }

    // create a matrix containing all numbers from 0 to width * height in random order
    private int[,] GenerateRandomMatrix(int width, int height) {
      int[,] matrix = new int[width, height];

      int row = 0;
      int col = 0;
      for (int i = 0; i < matrix.Length; i++) {
        matrix[col, row] = i;
        col++;
        if (col >= width) { col = 0; row++; }
      }
      RandomizeMatrix(matrix);  // shuffle
      return matrix;
    }

    // shuffle values of the matrix
    private void RandomizeMatrix(int[,] matrix) {
      // perform Knuth shuffle (http://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle)
      int width = matrix.GetLength(0);
      int height = matrix.GetLength(1);
      int temp, selectedRow, selectedCol;

      int row = 0;
      int col = 0;
      for (int i = 0; i < height * width; i++) {
        temp = matrix[col, row];

        // select random element from remaining elements
        // already processed elements must not be chosen a second time
        selectedRow = random.Next(row, height);
        if (selectedRow == row) selectedCol = random.Next(col, width);  // current row selected -> select from remaining columns
        else selectedCol = random.Next(width);  // new row selected -> select any column

        // swap
        matrix[col, row] = matrix[selectedCol, selectedRow];
        matrix[selectedCol, selectedRow] = temp;

        // incremet col and row
        col++;
        if (col >= width) { col = 0; row++; }
      }
    }
  }
}
