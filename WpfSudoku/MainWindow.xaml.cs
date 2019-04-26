using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using SudokuAlgorithm;

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
            var keyEvent = e as KeyEventArgs;
            var key = keyEvent.Key;
            if (key == Key.D0 || key == Key.D1 || key == Key.D2 || key == Key.D3 || key == Key.D4 || key == Key.D5
             || key == Key.D6 || key == Key.D7 || key == Key.D8 || key == Key.D9 || key == Key.NumPad0 || key == Key.NumPad1
             || key == Key.NumPad2 || key == Key.NumPad3 || key == Key.NumPad4 || key == Key.NumPad5 || key == Key.NumPad6
             || key == Key.NumPad7 || key == Key.NumPad8 || key == Key.NumPad9)
            {
                if (_this.SelectedText.Length == 1 || _this.Text.Length == 00)
                {
                    e.Handled = false;
                }
                else { e.Handled = true; }
            }
            else if (key == Key.Tab)
            {
                e.Handled = false;
            }
            else { e.Handled = true; }
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
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void SolveButtonClick(object sender, RoutedEventArgs e)
        {
            viewModel.Set(RecursiveBacktrackingSolver.solve(viewModel.Grid));
        }

        private void ResetButtonClick(object sender, RoutedEventArgs e)
        {
            viewModel.ResetAll();
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
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

    }
}
