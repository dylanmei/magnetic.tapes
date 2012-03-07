using System;
using NUnit.Framework;
using Magnetic.Tapes;

namespace Magnetic.Example.Tests.RSS
{
	[SetUpFixture]
	public class Setup
	{
		[SetUp]
		public void Init() {
			TapeMachine.Startup(config => {
				config.LibraryPath = "Tapes";
			});
		}

		[TearDown]
		public void Teardown() {
			TapeMachine.Shutdown();
		}
	}
}

