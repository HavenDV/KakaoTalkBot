namespace BotLibrary.Utilities
{
    public class Offsets
    {
        #region Static methods

        public static Offsets FromText(string text)
        {
            if (text.Contains(","))
            {
                var values = text.Split(',');
                int.TryParse(values[0], out var left);
                int.TryParse(values[1], out var top);
                int.TryParse(values[2], out var right);
                int.TryParse(values[3], out var bottom);

                return new Offsets(left, top, right, bottom);
            }

            int.TryParse(text, out var all);
            return new Offsets(all, all, all, all);
        }

        #endregion

        #region Properties

        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }

        #endregion

        #region Constructors

        public Offsets()
        {
        }

        public Offsets(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return $"{Left}, {Top}, {Right}, {Bottom}";
        }

        #endregion
    }
}
