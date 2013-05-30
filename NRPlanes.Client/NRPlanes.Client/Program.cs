using System;
using System.IO;

namespace NRPlanes.Client
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
#if !DEBUG
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
#else
            using (PlanesGame game = new PlanesGame())
            {
                game.Run();
            }
#endif
        }
    }
#endif
}

