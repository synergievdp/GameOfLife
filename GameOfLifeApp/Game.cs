using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GameOfLifeApp {
    class Game {
        public bool[][] Grid { get; private set; }
        public List<bool[][]> States { get; private set; }

        public Game(int height, int width) {
            CreateGrid(height, width);

            States.Add(Grid);
        }

        public void ChangeCell(int x, int y) {
            Grid[y][x] = !Grid[y][x];
        }

        public bool HasState(bool[][] other) {
            bool equal = true;
            foreach(bool[][] state in States) {
                equal = true;

                for(int y = 0; y < Math.Min(state.Length, other.Length); y++) {
                    for(int x = 0; x < Math.Min(state[y].Length, other[y].Length); x++) {
                        if (state[y][x] != other[y][x]) {
                            equal = false;
                            break;
                        }
                    }
                    if (!equal)
                        break;
                }

                if (equal)
                    break;
            }
            return equal;
        }

        public IEnumerable<(int, int)> ChangeState() {
            int[][] directions = {
                new int[]{-1, -1},  new int[]{-1, 0 },  new int[]{-1, 1},
                new int[]{0, -1 },                      new int[]{0, 1 },
                new int[]{1, -1},   new int[]{1, 0 },   new int[]{1, 1 }
            };

            bool[][] newState = new bool[Grid.Length][];
            for(int y = 0; y < Grid.Length; y++) {
                newState[y] = new bool[Grid[y].Length];
                for(int x = 0; x < Grid[y].Length; x++) {

                    int neighbours = 0;
                    foreach(int[] coord in directions) {
                        int ycoord = y + coord[0];
                        int xcoord = x + coord[1];
                        //Debug.WriteLine($"{ycoord >= 0 && ycoord < Grid.Length}, {xcoord >= 0 && xcoord < Grid[y].Length}");
                        if (ycoord >= 0 && ycoord < Grid.Length && xcoord >= 0 && xcoord < Grid[y].Length) {
                            //Debug.WriteLine(Grid[ycoord][xcoord]);
                            if (Grid[ycoord][xcoord] == true)
                                neighbours++;
                        }
                    }
                    //Debug.WriteLine($"({x},{y}) has {neighbours} neighbours");

                    if (Grid[y][x] == false && neighbours == 3) {
                        newState[y][x] = true;
                        yield return (y, x);
                    } else if (Grid[y][x] == true && (neighbours <= 1 || neighbours >= 4)) {
                        newState[y][x] = false;
                        yield return (y, x);
                    } else
                        newState[y][x] = Grid[y][x];

                }
            }

            States.Add(newState);
            Grid = newState;
        }

        private void CreateGrid(int height = 10, int width = 10) {
            height = height <= 0 ? 10 : height;
            width = width <= 0 ? 10 : width;

            Grid = new bool[height][];
            for (int y = 0; y < height; y++) {
                Grid[y] = new bool[width];
            }

            States = new();
        }

        public void FillGrid(bool[][] grid) {
            CreateGrid(grid.Length, grid.Max(row => row.Length));
            Grid = grid;

            States.Add(Grid);
        }

        public void ResizeGrid(int height, int width) {
            bool[][] newGrid = new bool[height][];

            for(int y = 0; y < height; y++) {
                newGrid[y] = new bool[width];
                if(y < Grid.Length) {
                    for(int x = 0; x < width; x++) {
                        if(x < Grid[y].Length) {
                            newGrid[y][x] = Grid[y][x];
                        }
                    }
                }
            }

            Grid = newGrid;
        }
    }
}
