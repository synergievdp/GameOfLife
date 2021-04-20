using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace GameOfLifeApp {
    class MainWindowViewModel : INotifyPropertyChanged {
        private DispatcherTimer timer;
        private Game game;

        public ObservableCollection<Cell> Cells { get; private set; } = new();
        private int interval = 168;
        public int Interval { 
            get { return interval; } 
            set {
                interval = value;
                timer.Interval = timer.Interval.TotalMilliseconds != interval ? new TimeSpan(0, 0, 0, 0, Interval) : timer.Interval; 
            } 
        }

        private int height = 10;
        public int Height { get { return height; } set { if (!timer.IsEnabled) { height = value; OnPropertyChanged(); } } }
        private int width = 10;
        public int Width { get { return width; } set { if (!timer.IsEnabled) { width = value; OnPropertyChanged(); } } }
        private int ticks = 0;
        public int Ticks { get { return ticks; } set { ticks = value; OnPropertyChanged(); } }

        public RelayCommand StartCmd { get; private set; }
        public RelayCommand StepCmd { get; private set; }
        public RelayCommand ResetCmd { get; private set; }
        public RelayCommand ChangeCmd { get; private set; }

        public MainWindowViewModel() {
            timer = new();
            timer.Interval = new TimeSpan(0, 0, 0, 0, Interval);
            timer.Tick += Step;

            StartCmd = new RelayCommand(_ => Start());
            StepCmd = new RelayCommand(_ => Step(null, EventArgs.Empty), _ => !timer.IsEnabled);
            ResetCmd = new RelayCommand(_ => Reset());
            ChangeCmd = new RelayCommand(cell => Change(cell as Cell)); //Disabled button style can't be changed from XAML?

            Reset();
        }

        private void Start() {
            if (timer.IsEnabled)
                timer.Stop();
            else timer.Start();

            StepCmd.RaiseCanExecuteChanged();
            ChangeCmd.RaiseCanExecuteChanged();

            //Debug.WriteLine("start");
        }

        private void Step(object sender, EventArgs e) {
            List<(int, int)> changes = game.ChangeState().ToList();
            
            if(timer.IsEnabled && changes.Count == 0)
                Start();

            foreach ((int y, int x) in changes) {
                int index = x + y * game.Grid[y].Length;
                //Debug.WriteLine($"Coordinate ({x}, {y}) is Cell ({Cells[index].X}, {Cells[index].Y}) at index ({index})");

                Cells[index].Alive = game.Grid[y][x];
            }

            Ticks++;

            //Debug.WriteLine("step");
        }

        private void Reset() {
            if (timer.IsEnabled)
                Start();

            game = new Game(Height, Width);

            Cells.Clear();
            for (int y = 0; y < game.Grid.Length; y++) {
                for (int x = 0; x < game.Grid[y].Length; x++) {
                    Cells.Add(new Cell(x, y, game.Grid[y][x]));
                }
            }

            Height = game.Grid.Length;
            Width = game.Grid.Max(row => row.Length);

            Ticks = 0;

            //Debug.WriteLine("reset");
        }

        private void Change(Cell cell) {
            if (timer.IsEnabled)
                return;

            game.ChangeCell(cell.X, cell.Y);

            int index = cell.X + cell.Y * game.Grid[cell.Y].Length;
            Cells[index].Alive = game.Grid[cell.Y][cell.X];

            //Debug.WriteLine($"Cell ({cell.X}, {cell.Y}) changed");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
