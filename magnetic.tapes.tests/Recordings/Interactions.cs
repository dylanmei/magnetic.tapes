using System;
using NUnit.Framework;
using Moq;

namespace Magnetic.Tapes.Tests
{
	public abstract class InteractionSpecification : Specification {
		protected Mock<ITape> tape;
		protected TapeRecorder recorder;
		protected Interaction interaction;
		
		protected override void Context() {
			tape = new Mock<ITape>();
			interaction = new Interaction();
			recorder = new TapeRecorder(tape.Object);
		}
		
		protected override void Because() {
			recorder.Record(interaction);
		}
	}
	
	[TestFixture]
	public class When_recording_an_interaction : InteractionSpecification {
		[Test]
		public void it_should_record_the_request() {
			tape.Verify(t => t.Write(interaction.Request), Times.Once());
		}
		
		[Test]
		public void it_should_record_the_response() {
			tape.Verify(t => t.Write(interaction.Response), Times.Once());
		}
	}
}

