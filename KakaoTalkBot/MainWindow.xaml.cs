using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using BotLibrary.Utilities;
using KakaoTalkBot.Actions;
using KakaoTalkBot.Extensions;
using KakaoTalkBot.Utilities;
using Microsoft.Win32;
using OfficeOpenXml;

namespace KakaoTalkBot
{
    public partial class MainWindow : IDisposable
    {
        #region Properties

        private static string ProcessName { get; } = "Nox";

        private ApplicationInfo Info { get; set; } = new ApplicationInfo();
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
            LoadScreens();
        }

        private (int main, int paste, int field) Timeouts => 
            (
                int.TryParse(BaseTimeoutTextBox.Text, out var result1) ? result1 : 1000,
                int.TryParse(PasteTimeoutTextBox.Text, out var result2) ? result2 : 1000,
                int.TryParse(FieldTimeoutTextBox.Text, out var result3) ? result3 : 1000);

        private void AddNumberButton_Click(object sender, RoutedEventArgs e)
        {
             MultiAction(() => AddFriend(NumberTextBox.Text, NumberTextBox.Text, CountryTextBox.Text, Timeouts));
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            MultiAction(() => SendMessage(NumberTextBox.Text, MessageTextBox.Text, Timeouts));
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

        private void LoadScreens() => SafeAction(nameof(LoadScreens), () =>
        {
            Info.Load("anchors");

            Log($"Screens count: {Info.Screens.Count}");
        });

        #endregion

        #region Utilities

        private void LogAppend(string text)
        {
            LogTextBox.Text += text;
            LogTextBox.InvalidateVisual();
            LogTextBox.Refresh();
        }

        private void Log(string text) => LogAppend(Environment.NewLine + text);

        private void Log(Exception exception) => Log($"Exception: {exception}");

        private void SafeAction(string name, Action action)
        {
            try
            {
                var watch = Stopwatch.StartNew();

                Log($"Action \"{name}\" started...");

                action?.Invoke();

                watch.Stop();
                var milliseconds = watch.ElapsedMilliseconds;

                Log($"Action \"{name}\" completed. Time: {milliseconds} ms");

                const int millisecondsInHour = 60 * 60 * 1000;
                var scope = millisecondsInHour / milliseconds;
                ScopeTextBlock.Text = $"{scope:F0} per hour";
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

        private void Action(string name, IAction action, (int main, int paste, int field) timeout) => SafeAction(name, () =>
        {
            action.Info = Info;
            action.PasteTimeout = timeout.paste;
            action.FieldClickTimeout = timeout.field;

            //WindowsUtilities.ShowWindow(ProcessName, 100);

            var watch = new Stopwatch();
            var isValid = true;
            while (isValid && !action.IsCompleted && IsStarted)
            {
                watch.Restart();
                Log($"Current action: {action.CurrentActionName}. Started... ");

                foreach (var (rect, mat) in ScreenshotUtilities.GetScreenshotOfProcess(ProcessName))
                {
                    MouseUtilities.GlobalOffset = (rect.X, rect.Y);
                    using (mat)
                    {
                        isValid = action.OnAction(mat);
                    }
                }

                LogAppend($"Ended. Elapsed ms: {watch.ElapsedMilliseconds}");

                Thread.Sleep(timeout.main);
            }
        });

        #endregion

        #region Actions

        private void SendMessage(string phone, string text, (int main, int paste, int field) timeouts) =>
            Action(nameof(SendMessage), new SendMessageAction { Phone = phone, Text = text }, timeouts);

        private void AddFriend(string name, string phone, string country, (int main, int paste, int field) timeouts) =>
            Action(nameof(SendMessage), new AddFriendAction { Name = name, Phone = phone, Country = country }, timeouts);

        #endregion

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Info.Dispose();
        }

        #endregion
    }
}
