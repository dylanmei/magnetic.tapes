using System;

namespace Magnetic.Tapes
{
	public class TapeRecorder {
		ITape tape;
		
		public TapeRecorder(ITape tape) {
			this.tape = tape;
		}

		public void Record(Interaction interaction) {
			tape.Write (interaction.Request);
			tape.Write (interaction.Response);
		}
	}
}

