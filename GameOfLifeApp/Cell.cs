using System.ComponentModel;
using System.Windows.Threading;

namespace GameOfLifeApp {
    class Cell : INotifyPropertyChanged {
        public int X { get; set; }
        public int Y { get; set; }
        private bool alive = false;
        public bool Alive { get { return alive; } set { alive = value; OnPropertyChanged(); } }

        public Cell(int x, int y, bool alive) {
            X = x;
            Y = y;
            Alive = alive;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
