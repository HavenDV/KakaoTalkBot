using System;
using System.Collections.Generic;

namespace BotLibrary.Extensions
{
    public static class EnumerableExtensions
    {
        public static void Dispose(this IEnumerable<IDisposable> collection)
        {
            foreach (var item in collection)
            {
                try
                {
                    item?.Dispose();
                }
                catch (Exception)
                {
                    // log exception and continue
                }
            }
        }
    }
}
