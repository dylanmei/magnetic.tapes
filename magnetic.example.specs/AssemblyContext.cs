using System;
using Magnetic.Tapes;
using Machine.Specifications;

namespace Magnetic.Example.Specs
{
	public class AssemblyContext : IAssemblyContext {
		public void OnAssemblyStart() {
			TapeMachine.Startup(config => {
				config.LibraryPath = "Tapes";
			});
		}

		public void OnAssemblyComplete() {
			TapeMachine.Shutdown();
		}
	}
}
