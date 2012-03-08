using System;

namespace Magnetic.Tapes
{
	public interface ITape {
		void Write(Request request);
		void Write(Response response);
	}
}

