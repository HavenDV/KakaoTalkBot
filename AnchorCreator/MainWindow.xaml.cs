using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using AnchorsCreator.Extensions;
using AnchorsCreator.Properties;
using KakaoTalkBotLibrary.Utilities;
using Ookii.Dialogs.Wpf;
using Point = System.Windows.Point;

namespace AnchorsCreator
{
    public partial class MainWindow : IDisposable
    {
        #region Properties

        private bool MouseIsDown { get; set; }
        private Point MouseDownPosition { get; set; }

        private Anchors Anchors { get; } = new Anchors();

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

        private void UpdateAnchors()
        {
            AnchorsCollection.Clear();
            if (!Anchors.Any())
            {
                return;
            }

            foreach (var pair in Anchors)
            {
                AnchorsCollection.Add(pair.Value);
            }

            AnchorsListBox.SelectedIndex = 0;
        }

        private void CloseImageButton_Click(object sender, RoutedEventArgs e)
        {
            SavePanel.Visibility = Visibility.Hidden;
            CloseImageButton.Visibility = Visibility.Hidden;
            Image.Source = null;
            SelectionRectangle.Visibility = Visibility.Hidden;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Anchors.Save(Settings.Default.CurrentDirectory);
        }

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

        private void AnchorsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var index = AnchorsListBox.SelectedIndex;
            if (index < 0)
            {
                return;
            }

            ShowAnchor(Anchors.ElementAt(index).Value);
        }

        #endregion

        #region Private methods

        private void Load(string directory)
        {
            if (!Directory.Exists(directory))
            {
                return;
            }

            Anchors.Load(directory);
            UpdateAnchors();

            if (!Anchors.Any())
            {
                return;
            }

            SavePanel.Visibility = Visibility.Visible;
        }

        private void ShowAnchor(Anchor anchor)
        {
            var image = anchor?.Mat?.Bitmap?.ToBitmapImage();
            if (image == null)
            {
                return;
            }

            CloseImageButton.Visibility = Visibility.Visible;
            Image.Source = image;
            ImageGrid.Width = image.Width;
            ImageGrid.Height = image.Height;

            var rectangle = Rectangle.Empty;
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
