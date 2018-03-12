using System;
using System.IO;
using System.Windows;
using Ookii.Dialogs.Wpf;

namespace AnchorsCreator.Utilities
{
    public static class MessageBoxUtilities
    {
        public static bool Question(string text, string title = "Question")
        {
            var result = MessageBox.Show(text, title, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);

            return result == MessageBoxResult.Yes;
        }

        public static bool? CancellableQuestion(string text, string title = "Question")
        {
            var result = MessageBox.Show(text, title, MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    return true;
                case MessageBoxResult.No:
                    return false;
                default:
                    return null;
            }
        }

        public static void Error(string text) => MessageBox.Show(text, @"Error", MessageBoxButton.OK, MessageBoxImage.Error);
        public static void Information(string text) => MessageBox.Show(text, @"Information", MessageBoxButton.OK, MessageBoxImage.Information);
        public static void Warning(string text) => MessageBox.Show(text, @"Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        public static void Exception(string text) => MessageBox.Show(text, @"Exception", MessageBoxButton.OK, MessageBoxImage.Error);
        public static void Exception(Exception exception) => Exception($@"Exception: {exception}");

        public static string OpenFolderDialog(string initialFolder = null)
        {
            var dialog = new VistaFolderBrowserDialog { ShowNewFolderButton = true };
            if (!string.IsNullOrEmpty(initialFolder) && Directory.Exists(initialFolder))
            {
                dialog.SelectedPath = initialFolder;
            }

            if (dialog.ShowDialog() != true)
            {
                return null;
            }

            return dialog.SelectedPath;
        }
    }
}
