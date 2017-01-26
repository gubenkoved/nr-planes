using System;
using System.IO;

namespace NRPlanes.Client.MonoGame
{
#if WINDOWS || LINUX
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
            try
            {
                using (PlanesGame game = new PlanesGame())
                {
                    game.Run();
                }
            }
            catch (Exception exc)
            {
                var fileStream = new FileStream("errors.txt", FileMode.Append);

                var streamWriter = new StreamWriter(fileStream);

                using (streamWriter)
                {
                    streamWriter.WriteLine(DateTime.Now);
                    streamWriter.WriteLine(exc.GetType().ToString());
                    streamWriter.WriteLine(exc.Message);
                    streamWriter.WriteLine(exc.StackTrace);
                    streamWriter.WriteLine(Environment.NewLine);
                }
            }
        }
    }
#endif
}
