using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SudokuAlgorithm;
using WpfSudoku.UnsafeSudokuSolver;

namespace WpfSudoku
{
    public class SingleDigitField : TextBox
    {
        public SingleDigitField()
        {
            AddHandler(PreviewMouseLeftButtonDownEvent,
              new MouseButtonEventHandler(SelectivelyIgnoreMouseButton), true);
            AddHandler(GotKeyboardFocusEvent,
              new RoutedEventHandler(SelectAllText), true);
            AddHandler(MouseDoubleClickEvent,
              new RoutedEventHandler(SelectAllText), true);
            AddHandler(KeyDownEvent, new RoutedEventHandler(RestrictEditing), true);
            this.Margin = new Thickness(3);
            this.TextAlignment = TextAlignment.Center;
        }

        private static void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            DependencyObject parent = e.OriginalSource as UIElement;
            while (parent != null && !(parent is TextBox))
                parent = VisualTreeHelper.GetParent(parent);

            if (parent != null)
            {
                var textBox = (TextBox)parent;
                if (!textBox.IsKeyboardFocusWithin)
                {
                    textBox.Focus();
                    e.Handled = true;
                }
            }
        }

        private static void RestrictEditing(object sender, RoutedEventArgs e)
        {
            var _this = e.OriginalSource as TextBox;
            switch ((e as KeyEventArgs).Key) {
                case Key.D7: case Key.D8: case Key.D9:
                case Key.D4: case Key.D5: case Key.D6:
                case Key.D1: case Key.D2: case Key.D3:
                case Key.D0:
                case Key.NumPad7: case Key.NumPad8: case Key.NumPad9:
                case Key.NumPad4: case Key.NumPad5: case Key.NumPad6:
                case Key.NumPad1: case Key.NumPad2: case Key.NumPad3:
                case Key.NumPad0:
                    if (_this.SelectedText.Length == 1 || _this.Text.Length == 00) {
                        e.Handled = false;
                    } else { e.Handled = true; }
                    break;
                case Key.Tab:
                    e.Handled = false;
                    break;
                default:
                    e.Handled = true;
                    break;
            }
        }

        private static void SelectAllText(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            if (textBox != null) textBox.SelectAll();
        }
    }

    public partial class MainWindow : Window
    {
        private SudokuViewModel viewModel = new SudokuViewModel();
        private Action SudokuAlgorithmWrapper;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = viewModel;
            this.SudokuAlgorithmWrapper = FSharpSolver(this.viewModel);
        }

        private Func<SudokuViewModel, Action> FSharpSolver = (SudokuViewModel viewModel) => () => {
            viewModel.Set(RecursiveBacktrackingSolver.solve(viewModel.Grid));
            MessageBox.Show("Solved with F sharp algorithm");
        };

        private Func<SudokuViewModel, Action> CSharpSolver = (SudokuViewModel viewModel) => () => {
            UnsafeSudokuModel model = new UnsafeSudokuModel(viewModel.Grid);
            viewModel.Set(model.Solve());
            MessageBox.Show("Solved with C sharp algorithm");
        };

        private void SolveButtonClick(object sender, RoutedEventArgs e)
        {
            SudokuAlgorithmWrapper();
        }

        private void ResetButtonClick(object sender, RoutedEventArgs e)
        {
            viewModel.ResetAll();
        }

        private void SelectAlgorithmClick(object sender, RoutedEventArgs e) {
            foreach(var item in this.SelectionMenu.Items.SourceCollection) {
                MenuItem menuItem = item as MenuItem;
                if (menuItem.Header != (sender as MenuItem).Header) menuItem.IsChecked = false;
            }
            switch((sender as MenuItem).Header) {
                case "F#":
                    this.SudokuAlgorithmWrapper = FSharpSolver(this.viewModel);
                    break;
                case "C# Unsafe Pointers":
                    this.SudokuAlgorithmWrapper = CSharpSolver(this.viewModel);
                    break;
                default:
                    break;
            }
        }
    }

    public class SudokuViewModel : INotifyPropertyChanged
    {
        private int[] _Grid = new int[81];
        public event PropertyChangedEventHandler PropertyChanged;

        public int[] Grid
        {
            get { return _Grid; }
            set
            {
                _Grid = value;
                OnPropertyChanged("Grid");
            }
        }
        public SudokuViewModel() { }

        public void Set(int[] arr)
        {
            Grid = arr;
        }

        public void ResetAll()
        {
            int[] newArr = new int[81];
            for (int i = 0; i < 81; i++)
            {
                newArr[i] = 0;
            }
            Grid = newArr;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(name));  
        }

    }

    
}
