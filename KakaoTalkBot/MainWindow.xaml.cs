using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using Emgu.CV;
using KakaoTalkBot.Extensions;
using KakaoTalkBot.Utilities;
using Pranas;

namespace KakaoTalkBot
{
    public partial class MainWindow
    {
        #region Properties

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Auth();
        }

        #endregion

        #region Private methods

        #region Anchors

        private void LoadAnchors() => SafeAction(() =>
        {
            Log("Loading anchors...");
            AnchorsDictionary = MatUtilities.LoadAnchors("anchors");

            Log($"Anchors are loaded. Count: {AnchorsDictionary.Count}");
        });

        private Mat GetAnchor(string name) => AnchorsDictionary.TryGetValue(name, out var result) ? result : null;

        #endregion

        #region Utilities

        private void Log(string text) => LogTextBox.Text += Environment.NewLine + text;
        private void Log(Exception exception) => Log($"Exception: {exception}");

        private void SafeAction(Action action)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception exception)
            {
                Log(exception);
            }
        }

        #endregion

        #region Find

        private (int, int, int, int) Find(IInputArray mat, string name)
        {
            using (var obj = GetAnchor(name).ToGray())
            {
                return MatUtilities.Find(mat, obj);
            }
        }

        #endregion

        #region Actions

        private void Auth() => SafeAction(() =>
        {
            Log("Auth started...");

            WindowsUtilities.ShowWindow("KakaoTalk", 100);

            using (var image = ScreenshotCapture.TakeScreenshot(true))
            using (var bitmap = new Bitmap(image))
            using (var mat = bitmap.ToMat().ToGray())
            {
                var (x, y, w, h) = Find(mat, "auth_logo.bmp");
                x += w / 2;
                y += h + 20;

                MouseUtilities.MoveAndClickAndPaste(x, y, EmailTextBox.Text, 50);
                y += 40;

                MouseUtilities.MoveAndClickAndPaste(x, y, PasswordTextBox.Text, 50);
                y += 40;

                MouseUtilities.MoveAndClick(x, y);
            }

            Log("Auth completed");
        });

        #endregion

        #endregion

    }
}
