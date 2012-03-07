using System;
using System.IO;
using System.Reflection;

namespace Magnetic.Tapes
{
	public static class TapeMachine
	{
		static TapeLibrary library;
		static Proxy proxy;
		
		public static void Startup(Action<TapeConfiguration> configurationHandler) {
			var configuration = new TapeConfiguration();
			if (configurationHandler != null)
				configurationHandler(configuration);
			
			ConfigureTapeLibrary(configuration.LibraryPath);
			StartProxy();
		}
		
		public static void Shutdown() {
			if (proxy != null) proxy.Close();
		}
		
		static void ConfigureTapeLibrary(string libraryPath) {

			if (!Path.IsPathRooted(libraryPath)) {
				var rootedPath = AppDomain.CurrentDomain.BaseDirectory;
				var directory = Path.GetDirectoryName(rootedPath);

				if (directory.EndsWith("bin"))
					directory = Path.GetDirectoryName(directory);

				libraryPath = Path.Combine(directory, libraryPath);
			}
			
			library = new TapeLibrary { Path = libraryPath };
		}
		
		static void StartProxy() {
			proxy = new Proxy(8911);
		}
	}
}

