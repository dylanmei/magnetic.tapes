using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Magnetic.Tapes
{
	class Proxy
	{
		List<ProxyClient> clients;
		Socket listener;
		bool shutdown;
		
		public Proxy(int port) {
			var address = IPAddress.Parse("0.0.0.0");
			
			listener = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			listener.Bind (new IPEndPoint(address, port));
			Start();
		}
		
		void Start() {
			clients = new List<ProxyClient>();

			listener.Listen(50);
			listener.BeginAccept(new AsyncCallback(Socket_Accepted), null);
		}
	
		void Socket_Accepted(IAsyncResult result) {
			if (shutdown) return;
			
			var socket = listener.EndAccept(result);
			var client = new ProxyClient(socket);
			
			try {
				client
					.Handshake(() => clients.Add(client))
					.Complete(() => clients.Remove(client));
			}
			finally {
				listener.BeginAccept(new AsyncCallback(Socket_Accepted), null);
			}
		}

		public void Close() {
			shutdown = true;
			clients.ForEach(client => client.Dispose());
			listener.Kill();
		}
	}
}

