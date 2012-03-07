using System;
using NUnit.Framework;
using Magnetic.Tapes;

namespace Magnetic.Example.Tests.RSS
{
	public class Setup
	{
		[TestFixtureSetUp]
		public void Init() {
			TapeMachine.Startup(config => {
				config.LibraryPath = "Tapes";
			});
		}

		[TestFixtureTearDown]
		public void Teardown() {
			TapeMachine.Shutdown();
		}
	}
}

