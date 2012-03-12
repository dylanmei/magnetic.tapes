using System;

namespace Magnetic.Tapes
{
	public class TapeRecorder {
		ITapeWriter tape;
		
		public TapeRecorder(ITapeWriter tape) {
			this.tape = tape;
		}

		public void Record(Interaction interaction) {
			tape.Write (interaction);
		}
	}
}

