using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GameOfLifeApp {
    class Game {
        public bool[][] Grid { get; set; }

        public Game(int height, int width) {
            CreateEmptyGrid(height, width);
        }

        public Game() {
            CreateEmptyGrid();
        }

        public void ChangeCell(int x, int y) {
            Grid[y][x] = !Grid[y][x];
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

            Grid = newState;
        }

        private void CreateEmptyGrid(int height, int width) {
            height = height <= 0 ? 10 : height;
            width = width <= 0 ? 10 : width;

            Grid = new bool[height][];
            for (int y = 0; y < height; y++) {
                Grid[y] = new bool[width];
            }
        }

        private void CreateEmptyGrid() {
            CreateEmptyGrid(10, 10);
        }
    }
}
