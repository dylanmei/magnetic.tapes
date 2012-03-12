using System;
using NUnit.Framework;
using Moq;

namespace Magnetic.Tapes.Tests
{
	public abstract class InteractionSpecification : Specification {
		protected Mock<ITapeWriter> tape;
		protected TapeRecorder recorder;
		protected Interaction interaction;
		
		protected override void Context() {
			tape = new Mock<ITapeWriter>();
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
		public void it_should_be_written_to_tape() {
			tape.Verify(t => t.Write(interaction), Times.Once());
		}
	}
}

