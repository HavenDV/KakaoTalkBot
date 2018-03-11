using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Emgu.CV;
using KakaoTalkBot.Actions;
using KakaoTalkBot.Extensions;
using KakaoTalkBot.Utilities;
using KakaoTalkBotLibrary.Extensions;
using KakaoTalkBotLibrary.Utilities;
using Microsoft.Win32;
using OfficeOpenXml;
using Pranas;

namespace KakaoTalkBot
{
    public partial class MainWindow : IDisposable
    {
        #region Properties

        private static string ProcessName { get; } = "Nox";

        private Dictionary<string, Mat> AnchorsDictionary { get; set; } = new Dictionary<string, Mat>();
        private Hook Hook { get; } = new Hook("Global Action Hook");

        private bool IsStarted { get; set; }

        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();

            #region Hook

            Hook.KeyUpEvent += Global_KeyUp;

            #endregion
        }

        #endregion

        #region Event handlers

        private void Global_KeyUp(KeyboardHookEventArgs e)
        {
            if (!IsStarted || (int)e.Key != 32)
            {
                return;
            }

            IsStarted = false;
            MessageBox.Show("Stopped");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                using (var image = ScreenshotCapture.TakeScreenshot(true))
                using (var bitmap = new Bitmap(image))
                using (var _ = bitmap.ToMat().ToGray())
                {
                }
            });

            LoadAnchors();
        }

        private void AddNumberButton_Click(object sender, RoutedEventArgs e)
        {
             MultiAction(() => AddFriend(NumberTextBox.Text, NumberTextBox.Text, CountryTextBox.Text));
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            MultiAction(() => SendMessage(NumberTextBox.Text, MessageTextBox.Text));
        }

        private async void LoadExcelButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() != true)
            {
                return;
            }

            var path = dialog.FileName;
            var items = await Task.Run(() => LoadExcel(path, 2, 6));

            ListView.Items.Clear();
            foreach (var item in items.Where(i => !string.IsNullOrWhiteSpace(i) && i.Length == 11))
            {
                ListView.Items.Add(item);
            }

            if (items.Length > 0)
            {
                ListView.SelectedIndex = 0;
            }
        }

        private void ListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            NumberTextBox.Text = ListView.SelectedItem.ToString();
        }

        private static string[] LoadExcel(string path, int worksheetIndex, int columnIndex)
        {
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets[worksheetIndex];
                var totalRows = worksheet.Dimension.End.Row;

                var items = new List<string>();
                for (var rowNum = 1; rowNum <= totalRows; rowNum++)
                {
                    var row = worksheet.Cells[rowNum, columnIndex, rowNum, columnIndex].Select(c => c.Value?.ToString() ?? string.Empty);
                    items.Add(string.Join(",", row));
                }

                return items.ToArray();
            }
        }

        #endregion

        #region Private methods

        #region Anchors

        private void LoadAnchors() => SafeAction(nameof(LoadAnchors), () =>
        {
            AnchorsDictionary = MatUtilities.LoadAnchors("anchors");

            Log($"Anchors count: {AnchorsDictionary.Count}");
        });

        #endregion

        #region Utilities

        private void Log(string text)
        {
            LogTextBox.Text += Environment.NewLine + text;
            LogTextBox.InvalidateVisual();
            LogTextBox.Refresh();
        }

        private void Log(Exception exception) => Log($"Exception: {exception}");

        private void SafeAction(string name, Action action)
        {
            try
            {
                Log($"Action \"{name}\" started...");

                action?.Invoke();

                Log($"Action \"{name}\" completed");
            }
            catch (Exception exception)
            {
                Log(exception);
            }
        }

        private void MultiAction(Action action)
        {
            var processes = Process.GetProcessesByName(ProcessName).Where(i => !string.IsNullOrWhiteSpace(i.MainWindowTitle));
            var process = processes.FirstOrDefault();
            if (process == null)
            {
                Log($"Process is not found: {ProcessName}");
                return;
            }

            if (ListView.Items.Count == 0)
            {
                IsStarted = true;
                action?.Invoke();
                IsStarted = false;
                return;
            } 

            IsStarted = true;
            while (IsStarted)
            {
                action?.Invoke();

                if (IsStarted)
                {
                    ++ListView.SelectedIndex;
                }
            }
        }

        private void Action(string name, IAction action) => SafeAction(name, () =>
        {
            action.AnchorsDictionary = AnchorsDictionary;

            WindowsUtilities.ShowWindow(ProcessName, 100);

            var isValid = true;
            while (isValid && !action.IsCompleted && IsStarted)
            {
                Log($"Current action: {action.CurrentActionName}");
                foreach (var (rect, mat) in ScreenshotUtilities.GetScreenshotOfProcess(ProcessName))
                {
                    MouseUtilities.GlobalOffset = (rect.X, rect.Y);
                    using (mat)
                    {
                        isValid = action.OnAction(mat);
                    }
                }

                Thread.Sleep(1000);
            }
        });

        #endregion

        #region Actions

        private void SendMessage(string phone, string text) =>
            Action(nameof(SendMessage), new SendMessageAction { Phone = phone, Text = text });

        private void AddFriend(string name, string phone, string country) =>
            Action(nameof(SendMessage), new AddFriendAction { Name = name, Phone = phone, Country = country });

        #endregion

        #endregion

        #region IDisposable

        public void Dispose()
        {
            foreach (var pair in AnchorsDictionary)
            {
                pair.Value.Dispose();
            }
            AnchorsDictionary.Clear();
        }

        #endregion
    }
}
