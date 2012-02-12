using System;

namespace ReleaseTest
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            using (ReleaseTestGame game = new ReleaseTestGame())
            {
                game.Run();
            }
        }
    }
#endif
}

