using System;

namespace Brownien {
    public static class Program {
        [STAThread]
        static void Main() {
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo) System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            using (var game = new Game1())
                game.Run();
        }
    }
}