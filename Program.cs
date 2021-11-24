using System;
using System.Globalization;
using System.Threading;

namespace Brownien {
    public static class Program {
        [STAThread]
        private static void Main() {
            var customCulture =
                (CultureInfo) Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            Thread.CurrentThread.CurrentCulture = customCulture;

            using var game = new Game1();
            game.Run();
        }
    }
}