using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using AnchorsCreator.Extensions;
using AnchorsCreator.Properties;
using AnchorsCreator.Utilities;
using BotLibrary.Utilities;
using Ookii.Dialogs.Wpf;
using Point = System.Windows.Point;

namespace AnchorsCreator.Forms
{
    public partial class MainWindow : IDisposable
    {
        #region Properties

        private bool MouseIsDown { get; set; }
        private Point MouseDownPosition { get; set; }

        public ApplicationInfo Info { get; set; } = new ApplicationInfo();

        private Screen CurrentScreen { get; set; }
        private Anchor CurrentAnchor { get; set; }

        public ObservableCollection<Screen> ScreensCollection { get; } = new ObservableCollection<Screen>();
        public ObservableCollection<Anchor> AnchorsCollection { get; } = new ObservableCollection<Anchor>();

        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();

            Load(Settings.Default.CurrentDirectory);
        }

        #endregion

        #region Event handlers

        private void AddScreenButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new SelectProcessWindow();
            if (window.ShowDialog() != true)
            {
                return;
            }

            var process = window.SelectedProcess;
            if (process == null)
            {
                return;
            }

            var (_, bitmap) = WindowsUtilities.GetScreenshotOfWindow(process.MainWindowHandle);
            if (bitmap == null)
            {
                MessageBox.Show("Error");
                return;
            }

            var path = Path.Combine(Settings.Default.CurrentDirectory, $"{process.ProcessName}_{new Random().Next()}.bmp");

            bitmap.Save(path, ImageFormat.Bmp);

            Load(Settings.Default.CurrentDirectory);
        }

        private void DeleteScreenButton_Click(object sender, RoutedEventArgs e)
        {
            var screen = GetSelectedScreen();
            if (!MessageBoxUtilities.Question($"Are you sure delete the screen: {screen.Name}?"))
            {
                return;
            }

            ScreensCollection.Remove(screen);
            Info.Screens.Remove(screen);

            var path = Path.Combine(Settings.Default.CurrentDirectory, screen.Name);
            File.Delete(path);

            Save();

            Load(Settings.Default.CurrentDirectory);
        }

        private void BrowseCurrentPathButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog
            {
                ShowNewFolderButton = true
            };

            if (Directory.Exists(CurrentPathTextBox.Text))
            {
                dialog.SelectedPath = CurrentPathTextBox.Text + @"\";
            }

            var result = dialog.ShowDialog();
            if (result != true)
            {
                return;
            }

            Settings.Default.CurrentDirectory = dialog.SelectedPath;
            Settings.Default.Save();

            Load(Settings.Default.CurrentDirectory);
        }

        private void UpdateScreens()
        {
            ScreensCollection.Clear();
            if (!Info.Screens.Any())
            {
                return;
            }

            foreach (var screen in Info.Screens)
            {
                ScreensCollection.Add(screen);
            }

            ScreensListBox.SelectedIndex = 0;
        }

        private void UpdateAnchors(int index = -1)
        {
            AnchorsCollection.Clear();
            if (!CurrentScreen.Anchors.Any())
            {
                return;
            }

            foreach (var anchor in CurrentScreen.Anchors)
            {
                AnchorsCollection.Add(anchor);
            }

            AnchorsListBox.SelectedIndex = index;
        }

        private void AddAnchorButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentScreen.Anchors.Add(new Anchor
            {
                Name = $"NewAnchor_{new Random().Next()}"
            });
            UpdateAnchors(CurrentScreen.Anchors.Count - 1);
        }

        private void DeleteAnchorButton_Click(object sender, RoutedEventArgs e)
        {
            var index = AnchorsListBox.SelectedIndex;
            if (index < 0)
            {
                return;
            }

            var anchor = AnchorsCollection[index];
            if (!MessageBoxUtilities.Question($"Are you sure delete the anchor: {anchor.Name}?"))
            {
                return;
            }

            AnchorsCollection.Remove(anchor);
            CurrentScreen.Anchors.Remove(anchor);
        }

        private Screen GetSelectedScreen()
        {
            var index = ScreensListBox.SelectedIndex;

            return index < 0 ? null : ScreensCollection[index];
        }

        private void EditScreenButton_Click(object sender, RoutedEventArgs e)
        {
            var screen = GetSelectedScreen();
            var window = new RenameWindow
            {
                OldName = screen.Name,
                NewName = screen.Name
            };
            if (window.ShowDialog() != true ||
                string.Equals(window.NewName, screen.Name, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var newName = window.NewName;
            var path = Path.Combine(Settings.Default.CurrentDirectory, screen.Name);
            var newPath = Path.Combine(Settings.Default.CurrentDirectory, newName);
            File.Copy(path, newPath);
            File.Delete(path);

            screen.Name = newName;
            Save();

            Load(Settings.Default.CurrentDirectory);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e) => Save();

        private void Grid_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MouseIsDown = false;
            ImageGrid.ReleaseMouseCapture();

            ChangeSelectionRectangle(MouseDownPosition, e.GetPosition(ImageGrid));
        }

        private void Grid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MouseIsDown = true;
            MouseDownPosition = e.GetPosition(ImageGrid);
            SelectionRectangle.Visibility = Visibility.Visible;
            ChangeSelectionRectangle(MouseDownPosition, MouseDownPosition);

            ImageGrid.CaptureMouse();
        }

        private void Grid_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!MouseIsDown)
            {
                return;
            }

            ChangeSelectionRectangle(MouseDownPosition, e.GetPosition(ImageGrid));
        }

        private void ScreensListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var index = ScreensListBox.SelectedIndex;

            ShowScreen(index < 0 ? null : Info.Screens.ElementAt(index));
        }

        private void AnchorsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var index = AnchorsListBox.SelectedIndex;

            ShowAnchor(index < 0 ? null : CurrentScreen.Anchors.ElementAt(index));
        }

        #endregion

        #region Private methods
        
        private void Save()
        {
            Info.Save(Settings.Default.CurrentDirectory);
        }

        private void Load(string directory)
        {
            if (!Directory.Exists(directory))
            {
                return;
            }

            Info.Load(directory);
            UpdateScreens();
        }

        private void ShowScreen(Screen screen)
        {
            CurrentScreen = screen;

            var image = screen?.Mat?.Bitmap?.ToBitmapImage();
            if (image == null)
            {
                return;
            }

            Image.Source = image;
            ImageGrid.Width = image.Width;
            ImageGrid.Height = image.Height;

            UpdateAnchors(0);
        }

        private void ShowAnchor(Anchor anchor)
        {
            CurrentAnchor = anchor;

            var rectangle = anchor?.Rectangle ?? Rectangle.Empty;
            if (rectangle.IsEmpty)
            {
                SelectionRectangle.Visibility = Visibility.Hidden;
                return;
            }

            SelectionRectangle.Visibility = Visibility.Visible;
            ChangeSelectionRectangle(rectangle);
        }

        private void ChangeSelectionRectangle(Point point1, Point point2)
        {
            var topLeftPoint = new Point(Math.Min(point1.X, point2.X), Math.Min(point1.Y, point2.Y));
            SelectionRectangle.Margin = new Thickness(topLeftPoint.X, topLeftPoint.Y, topLeftPoint.X, topLeftPoint.Y);
            SelectionRectangle.Width = Math.Abs(point2.X - point1.X);
            SelectionRectangle.Height = Math.Abs(point2.Y - point1.Y);

            if (CurrentAnchor == null)
            {
                return;
            }

            CurrentAnchor.Rectangle = new Rectangle((int)SelectionRectangle.Margin.Left, (int)SelectionRectangle.Margin.Top,
                (int)SelectionRectangle.Width, (int)SelectionRectangle.Height);
        }

        private void ChangeSelectionRectangle(Rectangle rectangle) => ChangeSelectionRectangle(
            new Point(rectangle.X, rectangle.Y),
            new Point(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height));

        #endregion

        #region IDisposable

        public void Dispose()
        {
            //Source?.Dispose();
            //Source = null;

        }

        #endregion
    }
}
