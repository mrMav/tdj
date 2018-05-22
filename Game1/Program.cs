using System;
using System.Globalization;
using System.Threading;

namespace TDJGame
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

            using (var game = new TDJGame())
                game.Run();
        }
    }
}
