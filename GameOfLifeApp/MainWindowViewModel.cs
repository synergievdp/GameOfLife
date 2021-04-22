using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace GameOfLifeApp {
    class MainWindowViewModel : INotifyPropertyChanged {
        private DispatcherTimer timer;
        private Game game;

        public ObservableCollection<Cell> Cells { get; } = new();
        public ObservableCollection<Pattern> Patterns { get; } = new();
        private int interval = 168;
        public int Interval {
            get { return interval; }
            set {
                interval = value;
                timer.Interval = timer.Interval.TotalMilliseconds != interval ? new TimeSpan(0, 0, 0, 0, Interval) : timer.Interval;
            }
        }

        private int height = 10;
        public int Height {
            get { return height; }
            set {
                if (!timer.IsEnabled) {
                    height = value;
                    OnPropertyChanged();
                    ChangeDimensions(Height, Width);
                }
            }
        }
        private int width = 10;
        public int Width {
            get { return width; }
            set {
                if (!timer.IsEnabled) {
                    width = value;
                    OnPropertyChanged();
                    ChangeDimensions(Height, Width);
                }
            }
        }
        private int ticks = 0;
        public int Ticks {
            get { return ticks; }
            set {
                ticks = value;
                OnPropertyChanged();
            }
        }
        private bool isPlaying;
        public bool IsPlaying { 
            get { return isPlaying; }
            set { 
                isPlaying = value;
                Ticks = 0;
                Changed = 0; 
                Visibility = isPlaying ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        private Visibility visibility = Visibility.Visible;
        public Visibility Visibility { 
            get { return visibility; }
            set { visibility = value; OnPropertyChanged(); }
        }
        private int changed;
        public int Changed {
            get { return changed; }
            set { changed = value; OnPropertyChanged(); }
        }
        private int currentBoardIndex = -1;

        public RelayCommand StartCmd { get; }
        public RelayCommand StepCmd { get; }
        public RelayCommand ClearCmd { get; }
        public RelayCommand ChangeCmd { get; }
        public RelayCommand SaveCmd { get; }
        public RelayCommand LoadCmd { get; }
        public RelayCommand DeleteCmd { get; }

        public MainWindowViewModel() {
            timer = new();
            timer.Interval = new TimeSpan(0, 0, 0, 0, Interval);
            timer.Tick += Step;

            StartCmd = new(_ => Start(), _ => !timer.IsEnabled);
            StepCmd = new(_ => Step(null, EventArgs.Empty), _ => !timer.IsEnabled);
            ClearCmd = new(_ => Clear());
            ChangeCmd = new(cell => Change(cell as Cell)); //Disabled button style can't be changed from XAML?
            SaveCmd = new(_ => SavePattern());
            LoadCmd = new(index => LoadPattern((int)index));
            DeleteCmd = new(index => DeletePattern((int)index));

            Clear();
            LoadFile();
        }

        private void SavePattern() {
            Patterns.Add(new Pattern(game.Grid));
            SaveFile();
        }

        private void SaveFile() {
            File.WriteAllText("patterns.json", JsonConvert.SerializeObject(Patterns, Formatting.Indented));
        }

        private void LoadFile() {
            Patterns.Clear();

            if (File.Exists("patterns.json")) {
                string file = File.ReadAllText("patterns.json");
                var patterns = JsonConvert.DeserializeObject<List<Pattern>>(file);
                if (patterns != null)
                    foreach (var pattern in patterns) {
                        Patterns.Add(pattern);
                    }
            }
        }

        private void LoadPattern(int index) {
            if (index > -1) {
                game.FillGrid(Patterns[index].Board);
                Rebuild();
                currentBoardIndex = index;
            }
        }

        private void DeletePattern(int index) {
            if (index > -1) {
                Patterns.RemoveAt(index);
            }

            SaveFile();
        }

        private void Start() {
            if (timer.IsEnabled)
                timer.Stop();
            else
                timer.Start();

            StepCmd.RaiseCanExecuteChanged();
            ChangeCmd.RaiseCanExecuteChanged();

            //Debug.WriteLine("start");
        }

        private void Step(object sender, EventArgs e) {
            List<(int, int)> changes = game.ChangeState().ToList();

            if (timer.IsEnabled && (game.HasState(game.Grid) || changes.Count == 0)) {
                Start();
                if (IsPlaying && currentBoardIndex > 0) {
                    StartCmd.RaiseCanExecuteChanged();
                    Patterns[currentBoardIndex].Highscores.Add(new Highscore(DateTime.Now.ToShortDateString(), Changed * Ticks));
                    SaveFile();
                }
            }

            foreach ((int y, int x) in changes) {
                int index = x + y * game.Grid[y].Length;
                //Debug.WriteLine($"Coordinate ({x}, {y}) is Cell ({Cells[index].X}, {Cells[index].Y}) at index ({index})");

                Cells[index].Alive = game.Grid[y][x];
            }

            Ticks++;

            //Debug.WriteLine("step");
        }

        private void Clear() {
            if (timer.IsEnabled)
                Start();

            game = new Game(Height, Width);

            Rebuild();

            Height = game.Grid.Length;
            Width = game.Grid.Max(row => row.Length);

            currentBoardIndex = -1;

            //Debug.WriteLine("reset");
        }

        private void ChangeDimensions(int height, int width) {
            game.ResizeGrid(height, width);
            Rebuild();

            currentBoardIndex = -1;
        }

        private void Rebuild() {
            Cells.Clear();
            for (int y = 0; y < game.Grid.Length; y++) {
                for (int x = 0; x < game.Grid[y].Length; x++) {
                    Cells.Add(new Cell(x, y, game.Grid[y][x]));
                }
            }

            Ticks = 0;
            Changed = 0;
        }

        private void Change(Cell cell) {
            if (timer.IsEnabled || (IsPlaying && game.Grid[cell.Y][cell.X]))
                return;

            game.ChangeCell(cell.X, cell.Y);

            int index = cell.X + cell.Y * game.Grid[cell.Y].Length;
            Cells[index].Alive = game.Grid[cell.Y][cell.X];

            Changed++;

            //Debug.WriteLine($"Cell ({cell.X}, {cell.Y}) changed");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
