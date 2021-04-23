using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLifeApp {
    class Pattern {
        public bool[][] Board { get; private set; }
        public List<(string, int)> Highscores { get; private set; } = new();

        public Pattern(bool[][] board) {
            Board = board;
        }
    }
}
