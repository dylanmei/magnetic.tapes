using System;
using NUnit.Framework;

namespace Magnetic.Tapes.Tests
{
	public abstract class Specification {

		[TestFixtureSetUp]
		public void Setup() {
			Context();
			Because();
		}
		
		protected abstract void Context();
		protected abstract void Because();

		[TestFixtureTearDown]
		public virtual void Teardown() {
		}
	}
}

