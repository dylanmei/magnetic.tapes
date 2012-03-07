using System;
using System.Net.Sockets;

namespace Magnetic.Tapes
{
	static class SocketExtensions
	{
		public static void Kill(this Socket socket) {
			try {
				if (socket.Connected)
					socket.Shutdown(SocketShutdown.Both);
			}
			catch {
			}
			finally {
				socket.Dispose();
			}
		}
	}
}

