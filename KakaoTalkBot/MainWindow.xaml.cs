using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Emgu.CV;
using KakaoTalkBot.Extensions;
using KakaoTalkBot.Utilities;
using Pranas;

namespace KakaoTalkBot
{
    public partial class MainWindow : IDisposable
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
            SendMessage(AddNumberTextBox.Text, "Text");
        }

        #endregion

        #region Private methods

        #region Anchors

        private void LoadAnchors() => SafeAction(nameof(LoadAnchors), () =>
        {
            AnchorsDictionary = MatUtilities.LoadAnchors("anchors");

            Log($"Anchors count: {AnchorsDictionary.Count}");
        });

        private Mat GetAnchor(string name) => AnchorsDictionary.TryGetValue(name, out var result) ? result : null;

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

        #endregion

        #region Find

        private (int, int, int, int) Find(IInputArray mat, string name)
        {
            return MatUtilities.Find(mat, GetAnchor(name));
        }

        #endregion

        #region Actions

        private void Auth() => SafeAction(nameof(Auth), () =>
        {
            WindowsUtilities.ShowWindow("KakaoTalk", 100);

            using (var mat = MatUtilities.GetScreenshot())
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
        });

        private void AddFriend(string name, string phone, string country) => SafeAction(nameof(AddFriend), () =>
        {
            WindowsUtilities.ShowWindow("BlueStacks", 100);

            using (var mat = MatUtilities.GetScreenshot())
            {
                var (x, y, w, h) = Find(mat, "add_by_contacts.bmp");
                x += w / 2;
                y += h / 2;

                MouseUtilities.MoveAndClick(x, y);
            }

            Thread.Sleep(500);

            using (var mat = MatUtilities.GetScreenshot())
            {
                ClipboardUtilities.Paste(name);

                Thread.Sleep(500);

                var (x, y, w, h) = Find(mat, "add_by_contacts_phone_code_field.bmp");
                x += w / 2;
                y += h / 2;

                Thread.Sleep(500);

                MouseUtilities.MoveAndClick(x, y);
            }

            Thread.Sleep(500);

            using (var mat = MatUtilities.GetScreenshot())
            {
                var (x, y, w, h) = Find(mat, "search_phone_code.bmp");
                x += w / 2;
                y += h / 2;

                MouseUtilities.MoveAndClick(x, y);

                ClipboardUtilities.Paste(country);

                Thread.Sleep(500);

                y += 12 * h;

                MouseUtilities.MoveAndClick(x, y);

                Thread.Sleep(500);

                ClipboardUtilities.Paste(phone);
            }

            Thread.Sleep(500);

            using (var mat = MatUtilities.GetScreenshot())
            {
                var (x, y, w, h) = Find(mat, "add_ok.bmp");
                x += w / 2;
                y += h / 2;

                MouseUtilities.MoveAndClick(x, y);
            }

            Thread.Sleep(1000);

            using (var mat = MatUtilities.GetScreenshot())
            {
                var (x, y, w, h) = Find(mat, "added_cancel.bmp");
                x += w / 2;
                y += h / 2;

                MouseUtilities.MoveAndClick(x, y);
            }
        });

        private void SendMessage(string phone, string text) => SafeAction(nameof(AddFriend), () =>
        {
            WindowsUtilities.ShowWindow("BlueStacks", 100);

            using (var mat = MatUtilities.GetScreenshot())
            {
                var (x, y, w, h) = Find(mat, "search.bmp");
                x += w / 2;
                y += h / 2;

                MouseUtilities.MoveAndClick(x, y);

                Thread.Sleep(1000);

                ClipboardUtilities.Paste(phone);

                Thread.Sleep(1000);

                y += (int)(2.5 * h);
                MouseUtilities.MoveAndClick(x, y);
            }

            Thread.Sleep(1000);

            using (var mat = MatUtilities.GetScreenshot())
            {
                var (x, y, w, h) = Find(mat, "free_chat.bmp");
                x += w / 2;
                y += h / 2;

                Thread.Sleep(1000);

                MouseUtilities.MoveAndClick(x, y);
            }
        });

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
