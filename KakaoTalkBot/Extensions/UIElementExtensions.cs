using System;
using System.Windows;
using System.Windows.Threading;

namespace KakaoTalkBot.Extensions
{
    public static class UiElementExtension
    {
        private static readonly Action EmptyDelegate = delegate { };

        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }
}
