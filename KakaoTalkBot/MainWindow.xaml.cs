using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Emgu.CV;
using KakaoTalkBot.Actions;
using KakaoTalkBot.Extensions;
using KakaoTalkBot.Utilities;
using Pranas;

namespace KakaoTalkBot
{
    public partial class MainWindow : IDisposable
    {
        #region Properties

        private static string ProcessName { get; } = "Nox";

        private Dictionary<string, Mat> AnchorsDictionary { get; set; } = new Dictionary<string, Mat>();

        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Event handlers

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
            AddFriend(NumberTextBox.Text, NumberTextBox.Text, "South Korea");
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            SendMessage(NumberTextBox.Text, MessageTextBox.Text);
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

        private void Log(string text) => LogTextBox.Text += Environment.NewLine + text;
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

        private void Action(string name, IAction action) => SafeAction(name, () =>
        {
            action.AnchorsDictionary = AnchorsDictionary;

            WindowsUtilities.ShowWindow(ProcessName, 100);

            var isValid = true;
            while (isValid && !action.IsCompleted)
            {
                Log($"Current action: {action.CurrentActionName}");
                foreach (var (rect, mat) in MatUtilities.GetScreenshotOfProcess(ProcessName))
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
            Action(nameof(SendMessage), new SendMessageAction{ Phone = phone, Text = text });

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
