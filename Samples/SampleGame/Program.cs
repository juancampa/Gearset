using System;

namespace SampleGame
{
#if WINDOWS || LINUX || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            using (var game = new MySampleGame())
            {
                game.Run();
            }
        }
    }

#elif MONOMAC

	using MonoMac.AppKit;
	using MonoMac.Foundation;

	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main (string[] args)
		{
			NSApplication.Init ();

			using (var p = new NSAutoreleasePool ()) {
				NSApplication.SharedApplication.Delegate = new AppDelegate ();
				NSApplication.Main (args);
			}
		}
	}

	class AppDelegate : NSApplicationDelegate
	{
		private static SampleGame.MySampleGame game;

		public override void FinishedLaunching (MonoMac.Foundation.NSObject notification)
		{
			// Handle a Xamarin.Mac Upgrade
			AppDomain.CurrentDomain.AssemblyResolve += (object sender, ResolveEventArgs a) => {
				if (a.Name.StartsWith ("MonoMac")) {
					return typeof(MonoMac.AppKit.AppKitFramework).Assembly;
				}
				return null;
			};
			game = new MySampleGame ();
			game.Run ();
		}

		public override bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication sender)
		{
			return true;
		}
	}
#endif
}