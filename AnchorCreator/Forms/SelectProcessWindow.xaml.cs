using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using AnchorsCreator.Extensions;
using BotLibrary.Utilities;

namespace AnchorsCreator.Forms
{
    public partial class SelectProcessWindow
    {
        public Process SelectedProcess { get; private set; }
        public IEnumerable<Tuple<Process, BitmapImage>> Processes => Process
            .GetProcesses()
            .Where(i => i.MainWindowHandle != IntPtr.Zero)
            .Select(i => new Tuple<Process, BitmapImage>(i, WindowsUtilities.GetScreenshotOfWindow(i.MainWindowHandle).Item2?.ToBitmapImage()));

        public SelectProcessWindow()
        {
            InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(e.Source is ListBox listBox))
            {
                return;
            }

            var index = listBox.SelectedIndex;
            if (index < 0)
            {
                return;
            }

            SelectedProcess = Processes.ElementAtOrDefault(index)?.Item1;
        }

        private void OkButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
